namespace SimpleBackup.StorageSources.S3.Settings
{
    public interface IAmazonStorageSettings
    {
        string AccessKey { get; }

        bool BackupEnabled { get; }

        string Bucket { get; }

        int NumberOfBackupsToKeep { get; }

        string Prefix { get; }

        string SecretAccessKey { get; }
    }
}