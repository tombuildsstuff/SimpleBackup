namespace SimpleBackup.StorageSources.LocalFileSystem.Settings
{
    public interface ILocalStorageSettings
    {
        string BackupDirectory { get; }

        bool BackupsEnabled { get; }

        int NumberOfBackupsToKeep { get; }
    }
}