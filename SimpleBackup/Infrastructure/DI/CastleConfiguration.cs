namespace SimpleBackup.Infrastructure.DI
{
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;

    using SimpleBackup.BackupSources.LocalFileSystem;
    using SimpleBackup.BackupSources.SqlServer;
    using SimpleBackup.Compressors.SevenZip;
    using SimpleBackup.Compressors.SevenZip.Settings;
    using SimpleBackup.Domain.Compression;
    using SimpleBackup.Domain.Engine;
    using SimpleBackup.Domain.Engine.Settings;
    using SimpleBackup.Domain.Logging;
    using SimpleBackup.Domain.Notifiers;
    using SimpleBackup.Domain.Notifiers.Email;
    using SimpleBackup.Domain.Notifiers.Email.Providers;
    using SimpleBackup.Domain.Notifiers.Email.Settings;
    using SimpleBackup.Domain.Sources;
    using SimpleBackup.Domain.Sources.Databases;
    using SimpleBackup.Domain.Sources.Files;
    using SimpleBackup.Domain.Storage;
    using SimpleBackup.Infrastructure.Logging;
    using SimpleBackup.Infrastructure.Runner;
    using SimpleBackup.StorageSources.LocalFileSystem;
    using SimpleBackup.StorageSources.LocalFileSystem.Settings;
    using SimpleBackup.StorageSources.S3;
    using SimpleBackup.StorageSources.S3.Settings;

    public class CastleConfiguration
    {
        public static IKernel Build()
        {
            var kernel = new DefaultKernel();
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel, true));

            RegisterBackupSources(kernel);
            RegisterCore(kernel);
            RegisterDatabaseProviders(kernel);
            RegisterEmail(kernel);
            RegisterLoggers(kernel);
            RegisterSevenZip(kernel);
            RegisterStorageSources(kernel);
            RegisterUserDataProviders(kernel);

            return kernel;
        }

        private static void RegisterCore(IKernel kernel)
        {
            kernel.Register(Component.For<IBackupEngine>().ImplementedBy<BackupEngine>());
            kernel.Register(Component.For<IBackupEngineSettings>().ImplementedBy<BackupEngineSettings>().LifestyleTransient());
            kernel.Register(Component.For<IConsoleBackupRunner>().ImplementedBy<ConsoleBackupRunner>().LifestyleTransient());
        }

        private static void RegisterBackupSources(IKernel kernel)
        {
            kernel.Register(Component.For<IBackupSource>().ImplementedBy<DatabasesBackupSource>().LifestyleTransient());
            kernel.Register(Component.For<IBackupSource>().ImplementedBy<FilesBackupSource>().LifestyleTransient());
        }

        private static void RegisterEmail(IKernel kernel)
        {
            kernel.Register(Component.For<IEmailSettings>().ImplementedBy<EmailSettings>().LifestyleTransient());
            kernel.Register(Component.For<ISmtpProvider>().ImplementedBy<SmtpProvider>().LifestyleTransient());
            kernel.Register(Component.For<IGetNotifiedWhenABackupIsCompleted>().ImplementedBy<EmailNotifier>().LifestyleTransient());
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

        private static void RegisterStorageSources(IKernel kernel)
        {
            kernel.Register(Component.For<ILocalStorageSettings>().ImplementedBy<LocalStorageSettings>().LifestyleTransient());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<LocalFileSystemStorageSource>().LifestyleTransient());

            kernel.Register(Component.For<IAmazonStorageSettings>().ImplementedBy<AmazonStorageSettings>().LifestyleTransient());
            kernel.Register(Component.For<IStorageSource>().ImplementedBy<AmazonS3StorageSource>().LifestyleTransient());
        }

        private static void RegisterUserDataProviders(IKernel kernel)
        {
            kernel.Register(Component.For<IBackupFiles>().ImplementedBy<LocalFileSystemBackupSource>());
        }
    }
}