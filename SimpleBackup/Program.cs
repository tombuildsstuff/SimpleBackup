namespace SimpleBackup
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Net;
	using System.Net.Mail;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Resolvers.SpecializedResolvers;

	using SimpleBackup.BackupSources.SqlServer;
	using SimpleBackup.Compressors.SevenZip;
	using SimpleBackup.Domain;
	using SimpleBackup.Domain.Databases;
	using SimpleBackup.Domain.Email;
	using SimpleBackup.Domain.Interfaces;
	using SimpleBackup.Domain.UserDefinedDirectories;
	using SimpleBackup.StorageSources.LocalFileSystem;
	using SimpleBackup.StorageSources.S3;

	public class Program
	{
		public static void Main(string[] args)
		{
			var kernel = BuildContainer();
			var engine = kernel.Resolve<BackupEngine>();
			var logger = kernel.Resolve<ILogger>();
			var outcomeNotifiers = kernel.ResolveAll<IOutcomeNotifier>();

			try
			{
				var settings = kernel.Resolve<ISettingsProvider>();
				var tempDirectory = settings.Get(ISettingsProvider.SettingsType.TempDirectory);
				var password = settings.Get(ISettingsProvider.SettingsType.Password);

				if (Directory.Exists(tempDirectory))
				{
					logger.Information("Temp Directory Exists - Deleting");
					Directory.Delete(tempDirectory, true);
				}

				logger.Information("Creating the Temp Directory");
				Directory.CreateDirectory(tempDirectory);

				logger.Information("Starting Backup");
				engine.RunBackup(logger, tempDirectory, password);
			}
			catch (Exception ex)
			{
				logger.Error(string.Format("Exception Running Backup {0}", ex.Message));
				logger.Error(ex.StackTrace);
			}

			var logFileName = string.Format("Log-{0}_{1}_{2}-{3}_{4}_{5}.log",
											DateTime.Now.Year,
											DateTime.Now.Month,
											DateTime.Now.Day,
											DateTime.Now.Hour,
											DateTime.Now.Minute,
											DateTime.Now.Second);
			logger.ExportToFile(logFileName);

			foreach (var outcomeNotifier in outcomeNotifiers)
			{
				Console.WriteLine("Sending log via {0}", outcomeNotifier.Name);
				var outcome = outcomeNotifier.Send(logFileName);
				Console.WriteLine("Sending {0}", outcome ? "Successful" : "Failed");
			}
		}

		private static IKernel BuildContainer()
		{
			var kernel = new DefaultKernel();
			kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));

			kernel.Register(Component.For<BackupEngine>().ImplementedBy<BackupEngine>());

			kernel.Register(Component.For<SevenZipConfiguration>().Instance(GetSevenZipConfiguration()).LifestyleTransient());
			kernel.Register(Component.For<EmailConfiguration>().Instance(GetEmailConfiguration()).LifestyleTransient());
			kernel.Register(Component.For<SmtpClient>().Instance(GetSmtpClient()).LifestyleTransient());

			//kernel.Register(Component.For<IDataCompressor>().ImplementedBy<ZIPDataCompressor>());
			kernel.Register(Component.For<IDataCompressor>().ImplementedBy<SevenZipDataCompressor>());
			kernel.Register(Component.For<IStorageSource>().ImplementedBy<S3StorageSource>().LifestyleTransient());
			kernel.Register(Component.For<IStorageSource>().ImplementedBy<LocalFileSystemStorageSource>().LifestyleTransient());
			kernel.Register(Component.For<ILogger>().Instance(ConfigureLogger()).LifestyleTransient());
			kernel.Register(Component.For<IOutcomeNotifier>().ImplementedBy<EmailOutcomeNotifier>().LifestyleTransient());

			// register each of the components here
			foreach (var userDefinedDirectory in GetAllUserDefinedDirectories())
				kernel.Register(
					Component.For<UserDefinedDirectory>().Instance(userDefinedDirectory).Named(userDefinedDirectory.FriendlyName));

			kernel.Register(Component.For<ISettingsProvider>().ImplementedBy<ConfigurationBasedSettingsProvider>().LifestyleTransient());
			var settings = kernel.Resolve<ISettingsProvider>();
			kernel.Register(Component.For<IDatabaseEngine>().Instance(ConfigureSQLServerEngine(settings)));
			kernel.Register(Component.For<LocalStorageSourceSettings>().Instance(ConfigureLocalProvider(settings)));
			kernel.Register(Component.For<CloudStorageSourceSettings>().Instance(ConfigureS3Provider(settings)));

			var assemblies = AllTypes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
			kernel.Register(assemblies.BasedOn<IBackupSource>().LifestyleTransient().WithService.FromInterface());

			return kernel;
		}

		private static EmailConfiguration GetEmailConfiguration()
		{
			var from = ConfigurationManager.AppSettings["SmtpSenderEmailAddress"];
			var fromAlias = ConfigurationManager.AppSettings["SmtpSenderEmailAlias"];
			var subject = ConfigurationManager.AppSettings["SmtpSubject"];
			var addresses = ConfigurationManager.AppSettings["SmtpToEmailAddresses"].Split(',');
			return new EmailConfiguration(from, fromAlias, subject, addresses);
		}

		private static SmtpClient GetSmtpClient()
		{
			var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"],int.Parse(ConfigurationManager.AppSettings["SmtpPort"]));
			client.UseDefaultCredentials = bool.Parse(ConfigurationManager.AppSettings["SmtpUseDefaultCredentials"]);
		    client.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["SmtpUseSsl"]);
            
			if (!client.UseDefaultCredentials)
			{
				var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpSenderEmailAddress"], ConfigurationManager.AppSettings["SmtpSenderPassword"]);
				client.Credentials = credentials;
			}

			return client;
		}

		private static SevenZipConfiguration GetSevenZipConfiguration()
		{
			return new SevenZipConfiguration(ConfigurationManager.AppSettings["SevenZipInstallationDirectory"]);
		}

		private static ILogger ConfigureLogger()
		{
			return new DualLogger(new InMemoryLogger(), new ConsoleLogger());
		}

		private static IEnumerable<UserDefinedDirectory> GetAllUserDefinedDirectories()
		{
			return ((UserDefinedDirectoryConfiguration) ConfigurationManager.GetSection("userDefinedDirectories")).DirectoryConfiguration;
		}

		private static IDatabaseEngine ConfigureSQLServerEngine(ISettingsProvider settings)
		{
			var server = settings.Get(ISettingsProvider.SettingsType.SQLServer);
			var authType = settings.Get(ISettingsProvider.SettingsType.SQLServerAuth);
			var timeout = int.Parse(settings.Get(ISettingsProvider.SettingsType.SQLServerTimeout));
			if (authType.Equals("SSPI", StringComparison.InvariantCultureIgnoreCase))
				return new SqlServerDatabaseEngine(server, timeout);

			if (!authType.Equals("Credentials", StringComparison.InvariantCultureIgnoreCase))
				throw new NotSupportedException("Unknown SQL Server Authentication Type");

			var user = settings.Get(ISettingsProvider.SettingsType.SQLServerUser);
			var pass = settings.Get(ISettingsProvider.SettingsType.SQLServerPass);
			return new SqlServerDatabaseEngine(server, user, pass, timeout);
		}

		private static LocalStorageSourceSettings ConfigureLocalProvider(ISettingsProvider settings)
		{
			var directory = settings.Get(ISettingsProvider.SettingsType.LocalBackupDirectory);
			var localBackupsToKeep = int.Parse(settings.Get(ISettingsProvider.SettingsType.LocalBackupsToKeep));
			return new LocalStorageSourceSettings(directory, localBackupsToKeep);
		}

		private static CloudStorageSourceSettings ConfigureS3Provider(ISettingsProvider settings)
		{
			var bucket = settings.Get(ISettingsProvider.SettingsType.S3Bucket);
			var accessKey = settings.Get(ISettingsProvider.SettingsType.S3AccessKey);
			var secretAccessKey = settings.Get(ISettingsProvider.SettingsType.S3SecretAccessKey);
			var prefix = settings.Get(ISettingsProvider.SettingsType.S3Prefix);
			var remoteBackupsToKeep = int.Parse(settings.Get(ISettingsProvider.SettingsType.RemoteBackupsToKeep));
			return new CloudStorageSourceSettings(bucket, accessKey, secretAccessKey, prefix, remoteBackupsToKeep);
		}
	}
}