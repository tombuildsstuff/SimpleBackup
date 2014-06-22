namespace SimpleBackup.Domain.Sources
{
    public interface IBackupSource
    {
        void BackupIntoDirectory(string directory);

        string Name { get; }
    }
}