namespace SimpleBackup
{
    using System.Collections.Generic;
    using System.IO;

    using SimpleBackup.Domain.Interfaces;

    public class InMemoryLogger : ILogger
    {
        private static List<string> _log;

        public InMemoryLogger()
        {
            _log = new List<string>();
        }

        public void Error(string message)
        {
            _log.Add(string.Format("**ERROR**: {0}", message));
        }

        public void Warning(string message)
        {
            _log.Add(string.Format("WARN: {0}", message));
        }

        public void Information(string message)
        {
            _log.Add(string.Format("Info: {0}", message));
        }

        public void ExportToFile(string file)
        {
            File.WriteAllLines(file, _log);
        }
    }
}