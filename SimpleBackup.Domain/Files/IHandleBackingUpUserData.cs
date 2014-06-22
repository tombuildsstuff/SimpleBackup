namespace SimpleBackup.Domain.Files
{
    public interface IHandleBackingUpUserData
    {
        bool BackupEnabled { get; }

        bool BackupIntoDirectory(string directory);

        string Name { get; }
    }
}