namespace SimpleBackup.Domain.Logging.File
{
    using System.Configuration;

    public class FileLoggerSettings : IFileLoggerSettings
    {
        public string LogsDirectory
        {
            get
            {
                return ConfigurationManager.AppSettings["LogsDirectory"];
            }
        }
    }
}