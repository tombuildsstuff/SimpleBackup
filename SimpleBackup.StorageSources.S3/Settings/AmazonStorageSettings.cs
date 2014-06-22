namespace SimpleBackup.StorageSources.S3.Settings
{
    using System.Configuration;

    public class AmazonStorageSettings : IAmazonStorageSettings
    {
        public string AccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Amazon.AccessKey"];
            }
        }

        public bool BackupEnabled
        {
            get
            {
                bool value;

                if (bool.TryParse(ConfigurationManager.AppSettings["Amazon.BackupEnabled"], out value))
                    return value;

                return false;
            }
        }

        public string Bucket
        {
            get
            {
                return ConfigurationManager.AppSettings["Amazon.Bucket"];
            }
        }

        public int NumberOfBackupsToKeep
        {
            get
            {
                int value;

                if (int.TryParse(ConfigurationManager.AppSettings["Amazon.NumberOfBackupsToKeep"], out value))
                    return value;

                return 7;
            }
        }

        public string Prefix
        {
            get
            {
                return ConfigurationManager.AppSettings["Amazon.Prefix"];
            }
        }

        public string SecretAccessKey
        {
            get
            {
                return ConfigurationManager.AppSettings["Amazon.SecretAccessKey"];
            }
        }
    }
}