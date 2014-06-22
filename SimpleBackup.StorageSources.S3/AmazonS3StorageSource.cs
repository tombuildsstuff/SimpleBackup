namespace SimpleBackup.StorageSources.S3
{
    using System.Collections.Generic;
    using System.Linq;

    using LitS3;

    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Storage;
    using SimpleBackup.StorageSources.S3.Settings;

    public class AmazonS3StorageSource : IStorageSource
    {
        private readonly IAmazonStorageSettings _settings;
        private readonly S3Service _service;

        public AmazonS3StorageSource(IAmazonStorageSettings settings)
        {
            _settings = settings;
            _service = new S3Service { AccessKeyID = _settings.AccessKey, SecretAccessKey = _settings.SecretAccessKey, UseSubdomains = true };
        }

        public void ArchiveBackup(BackupDetails details, string inputFile)
        {
			_service.AddObject(inputFile, _settings.Bucket, GenerateFileName(details));
        }

        public bool Enabled
        {
            get
            {
                return _settings.BackupEnabled;
            }
        }

        public string Name
        {
            get
            {
                return "Amazon S3";
            }
        }

        public IEnumerable<BackupDetails> RemoveOldBackups()
        {
            var files = GetAll().OrderBy(f => f.BackupDate).ToList();
            if (_settings.NumberOfBackupsToKeep >= files.Count())
                return null;

            var filesToDelete = files.Take(files.Count - _settings.NumberOfBackupsToKeep).ToList();
            foreach (var fileToDelete in filesToDelete)
                _service.DeleteObject(_settings.Bucket, GenerateFileName(fileToDelete));
            
            return filesToDelete;
        }

        private string GenerateFileName(BackupDetails details)
        {
            return string.Concat(_settings.Prefix, "/", details.GenerateFileName());
        }

        private IEnumerable<BackupDetails> GetAll()
        {
            // Common Prefix == Directory
            var files = _service.ListAllObjects(_settings.Bucket, string.Format("{0}/", _settings.Prefix)).Where(s => s as CommonPrefix == null && !string.IsNullOrWhiteSpace(s.Name)).ToList();
            return files.Select(f => BackupDetails.ParseFromBackupFile(f.Name)).ToList();
        }
    }
}