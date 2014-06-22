namespace SimpleBackup.Infrastructure.DI
{
    using System.Configuration;
    using System.Net;
    using System.Net.Mail;

    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;

    using SimpleBackup.BackupSources.LocalFileSystem;
    using SimpleBackup.BackupSources.SqlServer;
    using SimpleBackup.Compressors.SevenZip;
    using SimpleBackup.Compressors.SevenZip.Settings;
    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Databases;
    using SimpleBackup.Domain.Email;
    using SimpleBackup.Domain.Email.Settings;
    using SimpleBackup.Domain.Files;
    using SimpleBackup.Domain.Interfaces;
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

            RegisterBackupSources(kernel);
            RegisterCore(kernel);
            RegisterDatabaseProviders(kernel);
            RegisterLoggers(kernel);
            RegisterSevenZip(kernel);
            RegisterUserDataProviders(kernel);

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
            kernel.Register(Component.For<IBackupSource>().ImplementedBy<DatabasesBackupSource>().LifestyleTransient());
            kernel.Register(Component.For<IBackupSource>().ImplementedBy<FilesBackupSource>().LifestyleTransient());
        }

        private static void RegisterLegacyJunk(IKernel kernel)
        {
            // Legacy to clean..
            kernel.Register(Component.For<IEmailSettings>().ImplementedBy<EmailSettings>().LifestyleTransient());
            kernel.Register(Component.For<SmtpClient>().Instance(GetSmtpClient()).LifestyleTransient());

            kernel.Register(Component.For<IStorageSource>().ImplementedBy<S3StorageSource>().LifestyleTransient());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<LocalFileSystemStorageSource>().LifestyleTransient());
            kernel.Register(Component.For<IGetNotifiedWhenABackupIsCompleted>().ImplementedBy<EmailNotifier>().LifestyleTransient());

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

        private static void RegisterDatabaseProviders(IKernel kernel)
        {
            kernel.Register(Component.For<IProvideDatabaseBackups>().ImplementedBy<SqlServerBackupSource>());
            kernel.Register(Component.For<IHandleDatabaseRestores>().ImplementedBy<SqlServerRestoreSource>());
        }

        private static void RegisterSevenZip(IKernel kernel)
        {
            kernel.Register(Component.For<ICompressData>().ImplementedBy<SevenZipDataCompressor>().LifestyleTransient());
            kernel.Register(Component.For<ISevenZipSettings>().ImplementedBy<SevenZipSettings>().LifestyleTransient());
        }

        private static void RegisterUserDataProviders(IKernel kernel)
        {
            kernel.Register(Component.For<IBackupFiles>().ImplementedBy<LocalFileSystemBackupSource>());
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