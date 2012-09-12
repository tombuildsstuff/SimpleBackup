namespace SimpleBackup.Domain.Interfaces
{
    using System.Collections.Generic;

    public interface IStorageSource
    {
        void ArchiveBackup(BackupDetails details, byte[] encryptedBytes);

        IEnumerable<BackupDetails> GetAll();

        byte[] Retrieve(BackupDetails details);

        IEnumerable<BackupDetails> RemoveOldBackups();

        string Name();
    }
}