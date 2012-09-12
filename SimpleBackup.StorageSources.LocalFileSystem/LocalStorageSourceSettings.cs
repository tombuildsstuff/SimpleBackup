namespace SimpleBackup.StorageSources.LocalFileSystem
{
    public class LocalStorageSourceSettings
    {
        public string BackupDirectory { get; set; }

        public int NumberOfBackupsToKeep { get; set; }

        public LocalStorageSourceSettings(string backupDirectory, int numberOfBackupsToKeep)
        {
            BackupDirectory = backupDirectory;
            NumberOfBackupsToKeep = numberOfBackupsToKeep;
        }
    }
}