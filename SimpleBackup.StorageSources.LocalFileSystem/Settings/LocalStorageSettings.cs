namespace SimpleBackup.StorageSources.LocalFileSystem.Settings
{
    using System.Configuration;

    public class LocalStorageSettings : ILocalStorageSettings
    {
        public string BackupDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["LocalStorage.BackupDirectory"];
            }
        }

        public bool BackupsEnabled
        {
            get 
            {
                bool value;

                if (bool.TryParse(ConfigurationManager.AppSettings["LocalStorage.BackupsEnabled"], out value))
                    return value;

                return false;
            }
        }

        public int NumberOfBackupsToKeep
        {
            get
            {
                int value;

                if (int.TryParse(ConfigurationManager.AppSettings["LocalStorage.NumberOfBackupsToKeep"], out value))
                    return value;

                return 7;
            }
        }
    }
}