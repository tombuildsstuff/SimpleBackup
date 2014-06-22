namespace SimpleBackup.Domain.Files
{
    public interface IBackupFiles
    {
        bool Enabled { get; }

        bool BackupIntoDirectory(string directory);

        string Name { get; }
    }
}