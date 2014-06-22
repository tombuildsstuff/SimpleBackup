namespace SimpleBackup.Domain.Storage
{
    using System.Collections.Generic;

    public interface IStorageSource
    {
        void ArchiveBackup(BackupDetails details, string fileName);

        bool Enabled { get; }

        string Name { get; }

        IEnumerable<BackupDetails> RemoveOldBackups();
    }
}