namespace SimpleBackup.StorageSources.LocalFileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Interfaces;

    public class LocalFileSystemStorageSource : IStorageSource
    {
        private readonly LocalStorageSourceSettings _settings;

        public LocalFileSystemStorageSource(LocalStorageSourceSettings settings)
        {
            _settings = settings;
        }

        public void ArchiveBackup(BackupDetails details, byte[] encryptedBytes)
        {
            var path = Path.Combine(_settings.BackupDirectory, details.GenerateFileName());
            File.WriteAllBytes(path, encryptedBytes);
        }

        public IEnumerable<BackupDetails> GetAll()
        {
            var files = Directory.EnumerateFiles(_settings.BackupDirectory, "*.zip");
            return files.Select(f => BackupDetails.ParseFromBackupFile(f.Replace(_settings.BackupDirectory, string.Empty))).Where(f => f != null).OrderBy(f => f.BackupDate).ToList();
        }

        public byte[] Retrieve(BackupDetails details)
        {
            var filePath = Path.Combine(_settings.BackupDirectory, details.GenerateFileName());
            if (!File.Exists(filePath))
                throw new ArgumentNullException("details", string.Format("File Not Found: {0}", filePath));

            return File.ReadAllBytes(filePath);
        }

        public IEnumerable<BackupDetails> RemoveOldBackups()
        {
            var files = Directory.EnumerateFiles(_settings.BackupDirectory, "*.zip");
            var details = files.Select(f => BackupDetails.ParseFromBackupFile(f.Replace(_settings.BackupDirectory, string.Empty))).Where(f => f != null).OrderBy(f => f.BackupDate).ToList();
            if (_settings.NumberOfBackupsToKeep >= details.Count)
                return null;

            var filesToDelete = details.Take(details.Count - _settings.NumberOfBackupsToKeep).ToList();
            foreach (var fileToDelete in filesToDelete)
                File.Delete(Path.Combine(_settings.BackupDirectory, fileToDelete.GenerateFileName()));

            return filesToDelete;
        }

        public string Name()
        {
            return "Local File System";
        }
    }
}