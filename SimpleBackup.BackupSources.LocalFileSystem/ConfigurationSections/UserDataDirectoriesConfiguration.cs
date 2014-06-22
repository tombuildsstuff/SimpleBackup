namespace SimpleBackup.BackupSources.LocalFileSystem.ConfigurationSections
{
    using SimpleBackup.BackupSources.LocalFileSystem.Entities;

    public class UserDataDirectoriesConfiguration
    {
        public UserDataDirectory[] Directories { get; set; }
    }
}