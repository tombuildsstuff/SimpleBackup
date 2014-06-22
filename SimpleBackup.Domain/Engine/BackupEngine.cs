namespace SimpleBackup.Domain.Engine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SimpleBackup.Domain.Compression;
    using SimpleBackup.Domain.Engine.Settings;
    using SimpleBackup.Domain.Logging;
    using SimpleBackup.Domain.Sources;
    using SimpleBackup.Domain.Storage;

    public class BackupEngine : IBackupEngine
    {
        private readonly IEnumerable<IBackupSource> _backupSources;
        private readonly ICompressData _compressor;
        private readonly IEnumerable<IStorageSource> _storageSource;
        private readonly IBackupEngineSettings _settings;
        private readonly ILogger _logger;

        public BackupEngine(IEnumerable<IBackupSource> backupSources, ICompressData compressor, IEnumerable<IStorageSource> storageSource, IBackupEngineSettings settings, ILogger logger)
        {
            _backupSources = backupSources;
            _compressor = compressor;
            _storageSource = storageSource;
	        _settings = settings;
            _logger = logger;
        }

        public bool RunBackup()
        {
            if (!BackupCanBePerformed())
                return false;

            var details = new BackupDetails { Name = "Full Backup", BackupDate = DateTime.Now };
            var directory = Path.Combine(_settings.TempDirectory, Guid.NewGuid().ToString());

            PerformBackup(directory);

            var archive = Path.Combine(_settings.TempDirectory, string.Format("{0}.7z", DateTime.Now.Ticks));
            _logger.Information(string.Format("Compressing and Encrypting Archive into {0}..", archive));
            _compressor.CompressDataInToFile(directory, _settings.Password, archive);
            
            MoveArchiveIntoStorage(details, archive);
            RemoveArchive(archive);
            RemoveExpiredBackups();
            Directory.Delete(_settings.TempDirectory, true);
            return true;
        }

        private void RemoveArchive(string filePath)
        {
            _logger.Information(string.Format("Removing Archive: {0}", filePath));
            bool success;

            try
            {
                File.Delete(filePath);
                success = File.Exists(filePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                success = false;
            }

            if (success)
                _logger.Information("Archive was removed successfully..");
            else
                _logger.Error("Archive was NOT removed!");
        }

        private bool BackupCanBePerformed()
        {
            if (_backupSources == null || !_backupSources.Any())
            {
                _logger.Error("No Backup Sources Found!");
                return false;
            }

            if (_storageSource == null || !_storageSource.Any() || !_storageSource.Any(ss => ss.Enabled))
            {
                _logger.Error("No enabled storage sources were found!");
                return false;
            }

            return true;
        }

        private void PerformBackup(string directory)
        {
            foreach (var backupSource in _backupSources)
            {
                var backupDirectory = Path.Combine(directory, backupSource.Name);
                _logger.Information(string.Format("Backing up {0} into {1}", backupSource.Name, backupDirectory));
                Directory.CreateDirectory(backupDirectory);
                backupSource.BackupIntoDirectory(backupDirectory);
            }
        }

        private void MoveArchiveIntoStorage(BackupDetails details, string archive)
        {
            _logger.Information("Moving Archive into Storage.");

            foreach (var storageSource in _storageSource)
            {
                if (!storageSource.Enabled)
                {
                    _logger.Information(string.Format("Storage Source '{0}' is disabled.. skipping..", storageSource.Name));
                    continue;
                }

                _logger.Information(string.Format("Uploading to {0}", storageSource.Name));
				storageSource.ArchiveBackup(details, archive);
            }

            _logger.Information("Completed Uploading Data.");
        }
        
        private void RemoveExpiredBackups()
        {
            foreach (var storageSource in _storageSource)
            {
                if (!storageSource.Enabled)
                {
                    _logger.Information(string.Format("Storage Source '{0}' is disabled.. skipping..", storageSource.Name));
                    continue;
                }

                _logger.Information(string.Format("Checking for and removing old Backups from {0}", storageSource.Name));
                var removedBackups = storageSource.RemoveOldBackups();
                
                if (removedBackups == null)
                {
                    _logger.Information(string.Format("No backups to delete for {0}", storageSource.Name));
                    continue;
                }
                
                var backups = removedBackups.ToList();
                _logger.Information(string.Format("{0} has {1} expired backups", storageSource.Name, backups.Count()));
                foreach (var removedBackup in backups)
                    _logger.Information(string.Format("Deleted: {0} from {1}", removedBackup.GenerateFileName(), storageSource.Name));
            }
        }
    }
}