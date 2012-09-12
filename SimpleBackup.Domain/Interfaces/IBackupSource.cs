namespace SimpleBackup.Domain.Interfaces
{
    public interface IBackupSource
    {
        void BackupIntoDirectory(string directory);

        string Name { get; }
    }
}