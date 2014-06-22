namespace SimpleBackup.BackupSources.LocalFileSystem.Settings
{
    using System.Collections.Generic;
    using System.Configuration;

    using SimpleBackup.BackupSources.LocalFileSystem.ConfigurationSections;
    using SimpleBackup.BackupSources.LocalFileSystem.Entities;

    public class UserDataSettings : IUserDataSettings
    {
        public bool BackupsEnabled
        {
            get
            {
                bool value;

                if (bool.TryParse(ConfigurationManager.AppSettings["UserData.BackupsEnabled"], out value))
                    return value;

                return false;
            }
        }

        public IEnumerable<UserDataDirectory> DirectoriesToBackup
        {
            get
            {
                var section = ConfigurationManager.GetSection("userDataDirectories");
                if (section != null)
                {
                    var config = section as UserDataDirectoriesConfiguration;
                    if (config != null)
                        return config.Directories;
                }

                return new List<UserDataDirectory>();
            }
        }
    }
}