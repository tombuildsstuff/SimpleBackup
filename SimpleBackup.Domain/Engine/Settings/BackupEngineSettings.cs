namespace SimpleBackup.Domain.Engine.Settings
{
    using System.Configuration;

    public class BackupEngineSettings : IBackupEngineSettings
    {
        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["ArchivePassword"];
            }
        }

        public string TempDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["TempDirectory"];
            }
        }
    }
}