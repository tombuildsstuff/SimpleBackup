namespace SimpleBackup.Domain.Files
{
    using System;
    using System.Collections.Generic;

    using SimpleBackup.Domain.Interfaces;

    public class FilesBackupSource : IBackupSource
    {
        private readonly IEnumerable<IBackupFiles> _fileBackupProviders;

        private readonly ILogger _logger;

        public FilesBackupSource(IEnumerable<IBackupFiles> fileBackupProviders, ILogger logger)
        {
            _fileBackupProviders = fileBackupProviders;
            _logger = logger;
        }

        public void BackupIntoDirectory(string directory)
        {
            foreach (var provider in _fileBackupProviders)
            {
                _logger.Information(string.Format("Current File Backup Source: {0}", provider.Name));

                if (!provider.Enabled)
                {
                    _logger.Information(string.Format("File Source {0} not configured for backup. Skipping..", provider.Name));
                    return;
                }

                _logger.Information(string.Format("Started backing up files using Provider '{0}' into directory '{1}'", provider.Name, directory));

                var success = false;
                try
                {
                    success = provider.BackupIntoDirectory(directory);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error backing up files using Provider '{0}' into directory '{1}'", provider.Name, directory));
                    _logger.Error(ex.ToString());
                }

                _logger.Information(string.Format("Ended backup up files using Provider '{0}' - status: '{1}'", provider.Name, success));
            }
        }

        public string Name
        {
            get
            {
                return "User Defined Directories";
            }
        }
    }
}