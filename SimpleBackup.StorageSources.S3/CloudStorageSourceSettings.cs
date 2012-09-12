namespace SimpleBackup.StorageSources.S3
{
    public class CloudStorageSourceSettings
    {
        public string Bucket { get; private set; }

        public string AccessKey { get; private set; }

        public string SecretAccessKey { get; private set; }

        public string Prefix { get; private set; }

        public int NumberOfBackupsToKeep { get; private set; }

        public CloudStorageSourceSettings(string bucket, string accessKey, string secretAccessKey, string prefix, int numberOfBackupsToKeep)
        {
            Bucket = bucket;
            AccessKey = accessKey;
            SecretAccessKey = secretAccessKey;
            Prefix = prefix;
            NumberOfBackupsToKeep = numberOfBackupsToKeep;
        }
    }
}