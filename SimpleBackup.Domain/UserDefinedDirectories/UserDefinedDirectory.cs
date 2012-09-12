namespace SimpleBackup.Domain.UserDefinedDirectories
{
    public class UserDefinedDirectory
    {
        public string FriendlyName { get; set; }

        public string Path { get; set; }

        public UserDefinedDirectory(string friendlyName, string path)
        {
            FriendlyName = friendlyName;
            Path = path;
        }
    }
}