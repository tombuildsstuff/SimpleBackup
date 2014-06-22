namespace SimpleBackup.Domain.Logging.File
{
    using System;
    using System.IO;

    public class FileLogger : ILogger
    {
        private readonly IFileLoggerSettings _settings;

        public FileLogger(IFileLoggerSettings settings)
        {
            _settings = settings;
        }

        public void Error(string message)
        {
            WriteToFile(string.Format("ERROR: {0}", message));
        }

        public void Warning(string message)
        {
            WriteToFile(string.Format("WARN: {0}", message));
        }

        public void Information(string message)
        {
            WriteToFile(string.Format("INFO: {0}", message));
        }

        private void WriteToFile(string message)
        {
            var filePath = string.Concat(_settings.LogsDirectory, Path.DirectorySeparatorChar, GetFileName());
            using (var file = File.AppendText(filePath))
            {
                file.WriteLine(message);
                file.Flush();
            }
        }

        private string GetFileName()
        {
            return string.Format("Backup-{0}_{1}_{2}-{3}_{4}_{5}.log",
                                 DateTime.Now.Year,
                                 DateTime.Now.Month,
                                 DateTime.Now.Day,
                                 DateTime.Now.Hour,
                                 DateTime.Now.Minute,
                                 DateTime.Now.Second);
        }
    }
}