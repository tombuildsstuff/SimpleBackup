namespace SimpleBackup.BackupSources.LocalFileSystem
{
    using System;
    using System.IO;
    using System.Linq;

    using SimpleBackup.BackupSources.LocalFileSystem.Constants;
    using SimpleBackup.BackupSources.LocalFileSystem.Settings;
    using SimpleBackup.Domain.Logging;
    using SimpleBackup.Domain.Sources.Files;

    public class LocalFileSystemBackupSource : IBackupFiles
    {
        private readonly IUserDataSettings _settings;
        private readonly ILogger _logger;

        public LocalFileSystemBackupSource(IUserDataSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public bool Enabled
        {
            get
            {
                return _settings.BackupsEnabled;
            }
        }

        public bool BackupIntoDirectory(string directory)
        {
            var successful = true;

            foreach (var directoryToBackup in _settings.DirectoriesToBackup)
            {
                try
                {
                    var directoryPath = string.Concat(directory, Path.DirectorySeparatorChar.ToString(), directoryToBackup.FriendlyName, Path.DirectorySeparatorChar.ToString());
                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    var childDirectories = Directory.EnumerateDirectories(directoryToBackup.Path, "*", SearchOption.AllDirectories);
                    foreach (var childDirectory in childDirectories)
                        Directory.CreateDirectory(childDirectory.Replace(directoryToBackup.Path, directoryPath));

                    var files = Directory.EnumerateFiles(directoryToBackup.Path, "*.*", SearchOption.AllDirectories).ToList();
                    foreach (var file in files)
                        File.Copy(file, file.Replace(directoryToBackup.Path, directoryPath), false);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Unable to backup '{0}' into '{1}'", directoryToBackup, directory));
                    _logger.Error(ex.ToString());
                    successful = false;
                }
            }

            return successful;
        }

        public string Name
        {
            get
            {
                return Names.ProviderName;
            }
        }
    }
}