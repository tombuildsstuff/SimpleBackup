namespace SimpleBackup.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SimpleBackup.Domain.Interfaces;

    public class BackupEngine
    {
        private readonly IEnumerable<IBackupSource> _backupSources;
        private readonly IDataCompressor _compressor;
        private readonly IEnumerable<IStorageSource> _storageSource;

        public BackupEngine(IEnumerable<IBackupSource> backupSources, IDataCompressor compressor, IEnumerable<IStorageSource> storageSource)
        {
            _backupSources = backupSources;
            _compressor = compressor;
            _storageSource = storageSource;
        }

        public void RunBackup(ILogger logger, string tempDirectory, string password)
        {
            if (_backupSources == null || !_backupSources.Any())
            {
                logger.Error("No Backup Sources Found!");
                return;
            }

            if (_storageSource == null || !_storageSource.Any())
            {
                logger.Error("No Storage Sources Found!");
                return;
            }

            var details = new BackupDetails { Name = "Full Backup", BackupDate = DateTime.Now };
            var directory = Path.Combine(tempDirectory, Guid.NewGuid().ToString());

            foreach (var backupSource in _backupSources)
            {
                var backupDirectory = Path.Combine(directory, backupSource.Name);
                logger.Information(string.Format("Backing up {0} into {1}", backupSource.Name, backupDirectory));
                Directory.CreateDirectory(backupDirectory);
                backupSource.BackupIntoDirectory(backupDirectory);
            }

            logger.Information("Compressing and Encrypting Archive..");
            var encryptedBytes = _compressor.CompressData(directory, password);

            foreach (var storageSource in _storageSource)
            {
                logger.Information(string.Format("Uploading to {0}", storageSource.Name()));
                storageSource.ArchiveBackup(details, encryptedBytes);
            }

            logger.Information("Completed Uploading Data");
            foreach (var storageSource in _storageSource)
            {
                logger.Information(string.Format("Checking for and removing old Backups from {0}", storageSource.Name()));
                var removedBackups = storageSource.RemoveOldBackups();
                
                if (removedBackups == null)
                {
                    logger.Information(string.Format("No backups to delete for {0}", storageSource.Name()));
                    continue;
                }
                
                var backups = removedBackups.ToList();
                logger.Information(string.Format("{0} has {1} expired backups", storageSource.Name(), backups.Count()));
                foreach (var removedBackup in backups)
                    logger.Information(string.Format("Deleted: {0} from {1}", removedBackup.GenerateFileName(), storageSource.Name()));
            }

            Directory.Delete(tempDirectory, true);
        }
    }
}