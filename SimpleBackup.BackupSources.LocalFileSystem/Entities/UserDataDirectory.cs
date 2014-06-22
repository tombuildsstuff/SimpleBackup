namespace SimpleBackup.BackupSources.LocalFileSystem.Entities
{
    public class UserDataDirectory
    {
        public string FriendlyName { get; set; }

        public string Path { get; set; }

        public UserDataDirectory(string friendlyName, string path)
        {
            FriendlyName = friendlyName;
            Path = path;
        }
    }
}