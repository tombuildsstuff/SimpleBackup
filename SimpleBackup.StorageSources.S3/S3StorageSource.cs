namespace SimpleBackup.StorageSources.S3
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using LitS3;

    using SimpleBackup.Domain;
    using SimpleBackup.Domain.Interfaces;

    public class S3StorageSource : IStorageSource
    {
        private readonly CloudStorageSourceSettings _settings;
        private readonly S3Service _service;

        public S3StorageSource(CloudStorageSourceSettings settings)
        {
            _settings = settings;
            _service = new S3Service { AccessKeyID = _settings.AccessKey, SecretAccessKey = _settings.SecretAccessKey, UseSubdomains = true };
        }

        public void ArchiveBackup(BackupDetails details, byte[] encryptedBytes)
        {
            using (var streamReader = new MemoryStream(encryptedBytes))
                _service.AddObject(streamReader, _settings.Bucket, GenerateFileName(details));
        }

        public IEnumerable<BackupDetails> GetAll()
        {
            // Common Prefix == Directory
            var files = _service.ListAllObjects(_settings.Bucket, string.Format("{0}/", _settings.Prefix)).Where(s => s as CommonPrefix == null && !string.IsNullOrWhiteSpace(s.Name)).ToList();
            return files.Select(f => BackupDetails.ParseFromBackupFile(f.Name)).ToList();
        }

        public byte[] Retrieve(BackupDetails details)
        {
            byte[] bytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var stream = _service.GetObjectStream(_settings.Bucket, GenerateFileName(details)))
                    stream.CopyTo(memoryStream);
                
                bytes = memoryStream.ToArray();
            }

            return bytes;
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

        public string Name()
        {
            return "Amazon S3";
        }

        private string GenerateFileName(BackupDetails details)
        {
            return string.Concat(_settings.Prefix, "/", details.GenerateFileName());
        }
    }
}