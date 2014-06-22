namespace SimpleBackup.BackupSources.LocalFileSystem.Settings
{
    using System.Collections.Generic;
    using System.Configuration;

    using SimpleBackup.BackupSources.LocalFileSystem.ConfigurationSections;
    using SimpleBackup.Domain.UserDefinedDirectories;

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

        public IList<UserDefinedDirectory> DirectoriesToBackup
        {
            get
            {
                var section = ConfigurationManager.GetSection("userDefinedDirectories");
                if (section != null)
                    return ((UserDefinedDirectoryConfiguration)section).DirectoryConfiguration;

                return new List<UserDefinedDirectory>();
            }
        }
    }
}