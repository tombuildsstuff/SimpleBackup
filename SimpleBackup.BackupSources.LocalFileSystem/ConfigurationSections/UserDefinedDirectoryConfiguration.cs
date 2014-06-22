namespace SimpleBackup.BackupSources.LocalFileSystem.ConfigurationSections
{
    using SimpleBackup.Domain.UserDefinedDirectories;

    public class UserDefinedDirectoryConfiguration
    {
        public UserDefinedDirectory[] DirectoryConfiguration { get; set; }
    }
}