namespace SimpleBackup.StorageSources.LocalFileSystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Storage;
    using SimpleBackup.StorageSources.LocalFileSystem.Settings;

    public class LocalFileSystemStorageSource : IStorageSource
    {
        private readonly ILocalStorageSettings _settings;

        public LocalFileSystemStorageSource(ILocalStorageSettings settings)
        {
            _settings = settings;
        }

        public void ArchiveBackup(BackupDetails details, string fileName)
        {
            var path = Path.Combine(_settings.BackupDirectory, details.GenerateFileName());
			File.Copy(fileName, path, true);
        }

        public bool Enabled
        {
            get
            {
                return _settings.BackupsEnabled;
            }
        }

        public string Name
        {
            get
            {
                return "Local File System";
            }
        }

        public IEnumerable<BackupDetails> RemoveOldBackups()
        {
            var files = Directory.EnumerateFiles(_settings.BackupDirectory, "*.backup");
            var details = files.Select(f => BackupDetails.ParseFromBackupFile(f.Replace(_settings.BackupDirectory, string.Empty))).Where(f => f != null).OrderBy(f => f.BackupDate).ToList();
            if (_settings.NumberOfBackupsToKeep >= details.Count)
                return null;

            var filesToDelete = details.Take(details.Count - _settings.NumberOfBackupsToKeep).ToList();
            foreach (var fileToDelete in filesToDelete)
                File.Delete(Path.Combine(_settings.BackupDirectory, fileToDelete.GenerateFileName()));

            return filesToDelete;
        }
    }
}