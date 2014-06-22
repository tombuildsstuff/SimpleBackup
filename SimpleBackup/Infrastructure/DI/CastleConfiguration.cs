namespace SimpleBackup.Infrastructure.DI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
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
    using SimpleBackup.Infrastructure.Logging;
    using SimpleBackup.Infrastructure.Runner;
    using SimpleBackup.Infrastructure.Settings;
    using SimpleBackup.StorageSources.LocalFileSystem;
    using SimpleBackup.StorageSources.S3;

    public class CastleConfiguration
    {
        public static IKernel Build()
        {
            var kernel = new DefaultKernel();
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));

            RegisterCore(kernel);

            RegisterBackupSources(kernel);
            RegisterDatabaseBackupProviders(kernel);
            RegisterDatabaseRestoreProviders(kernel);
            RegisterLoggers(kernel);

            RegisterLegacyJunk(kernel);

            return kernel;
        }

        private static void RegisterCore(IKernel kernel)
        {
            kernel.Register(Component.For<BackupEngine>().ImplementedBy<BackupEngine>());
            kernel.Register(Component.For<IConsoleBackupRunner>().ImplementedBy<ConsoleBackupRunner>().LifestyleTransient());
        }

        private static void RegisterBackupSources(IKernel kernel)
        {
            var assemblies = AllTypes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            kernel.Register(assemblies.BasedOn<IBackupSource>().LifestyleTransient().WithService.FromInterface());
        }

        private static void RegisterLegacyJunk(IKernel kernel)
        {
            // Legacy to clean..
            kernel.Register(Component.For<SevenZipConfiguration>().Instance(GetSevenZipConfiguration()).LifestyleTransient());
            kernel.Register(Component.For<IEmailSettings>().ImplementedBy<EmailSettings>().LifestyleTransient());
            kernel.Register(Component.For<SmtpClient>().Instance(GetSmtpClient()).LifestyleTransient());

            kernel.Register(Component.For<IDataCompressor>().ImplementedBy<SevenZipDataCompressor>());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<S3StorageSource>().LifestyleTransient());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<LocalFileSystemStorageSource>().LifestyleTransient());
            kernel.Register(Component.For<IOutcomeNotifier>().ImplementedBy<EmailOutcomeNotifier>().LifestyleTransient());

            // register each of the components here
            foreach (var userDefinedDirectory in GetAllUserDefinedDirectories())
                kernel.Register(Component.For<UserDefinedDirectory>().Instance(userDefinedDirectory).Named(userDefinedDirectory.FriendlyName));

            kernel.Register(Component.For<ISettingsProvider>().ImplementedBy<ConfigurationBasedSettingsProvider>().LifestyleTransient());
            var settings = kernel.Resolve<ISettingsProvider>();
            kernel.Register(Component.For<LocalStorageSourceSettings>().Instance(ConfigureLocalProvider(settings)));
            kernel.Register(Component.For<CloudStorageSourceSettings>().Instance(ConfigureS3Provider(settings)));
        }

        private static void RegisterLoggers(IKernel kernel)
        {
            kernel.Register(Component.For<ILogger>().ImplementedBy<ConsoleLogger>().LifestyleTransient());
            kernel.Register(Component.For<ILogger>().ImplementedBy<InMemoryLogger>().LifestyleTransient());
            kernel.Register(Component.For<ILogger>().ImplementedBy<MultiLogger>().LifestyleTransient());
        }

        private static void RegisterDatabaseBackupProviders(IKernel kernel)
        {
            kernel.Register(Component.For<IProvideDatabaseBackups>().ImplementedBy<SqlServerBackupSource>());
        }

        private static void RegisterDatabaseRestoreProviders(IKernel kernel)
        {
        }

        private static SmtpClient GetSmtpClient()
        {
            var client = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpPort"]));
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

        private static IEnumerable<UserDefinedDirectory> GetAllUserDefinedDirectories()
        {
            return ((UserDefinedDirectoryConfiguration)ConfigurationManager.GetSection("userDefinedDirectories")).DirectoryConfiguration;
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