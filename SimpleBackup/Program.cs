namespace SimpleBackup
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;

    using SimpleBackup.BackupSources.SqlServer;
    using SimpleBackup.Compressors.SharpZipLib;
    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Databases;
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

            logger.ExportToFile(string.Format("Log-{0}_{1}_{2}-{3}_{4}_{5}.log", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second));
        }

        private static IKernel BuildContainer()
        {
            var kernel = new DefaultKernel();
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));

            kernel.Register(Component.For<BackupEngine>().ImplementedBy<BackupEngine>());
            kernel.Register(Component.For<IDataCompressor>().ImplementedBy<ZIPDataCompressor>());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<S3StorageSource>().LifestyleTransient());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<LocalFileSystemStorageSource>().LifestyleTransient());
            kernel.Register(Component.For<ILogger>().ImplementedBy<InMemoryLogger>().LifestyleTransient());

            // register each of the components here
            foreach (var userDefinedDirectory in GetAllUserDefinedDirectories())
                kernel.Register(Component.For<UserDefinedDirectory>().Instance(userDefinedDirectory).Named(userDefinedDirectory.FriendlyName));

            kernel.Register(Component.For<ISettingsProvider>().ImplementedBy<ConfigurationBasedSettingsProvider>().LifestyleTransient());
            var settings = kernel.Resolve<ISettingsProvider>();
            kernel.Register(Component.For<IDatabaseEngine>().Instance(ConfigureSQLServerEngine(settings)));
            kernel.Register(Component.For<LocalStorageSourceSettings>().Instance(ConfigureLocalProvider(settings)));
            kernel.Register(Component.For<CloudStorageSourceSettings>().Instance(ConfigureS3Provider(settings)));
            
            var assemblies = AllTypes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            kernel.Register(assemblies.BasedOn<IBackupSource>().LifestyleTransient().WithService.FromInterface());
            
            return kernel;
        }

        private static IEnumerable<UserDefinedDirectory> GetAllUserDefinedDirectories()
        {
            return ((UserDefinedDirectoryConfiguration)ConfigurationManager.GetSection("userDefinedDirectories")).DirectoryConfiguration;
        }

        private static IDatabaseEngine ConfigureSQLServerEngine(ISettingsProvider settings)
        {
            var server = settings.Get(ISettingsProvider.SettingsType.SQLServer);
            var authType = settings.Get(ISettingsProvider.SettingsType.SQLServerAuth);
            if (authType.Equals("SSPI", StringComparison.InvariantCultureIgnoreCase))
                return new SqlServerDatabaseEngine(server);

            if (!authType.Equals("Credentials", StringComparison.InvariantCultureIgnoreCase))
                throw new NotSupportedException("Unknown SQL Server Authentication Type");

            var user = settings.Get(ISettingsProvider.SettingsType.SQLServerUser);
            var pass = settings.Get(ISettingsProvider.SettingsType.SQLServerPass);
            return new SqlServerDatabaseEngine(server, user, pass);
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