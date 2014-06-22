namespace SimpleBackup.Domain.Logging
{
    public interface ILogger
    {
        void Error(string message);
        
        void Warning(string message);
        
        void Information(string message);

        void ExportToFile(string file);
    }
}