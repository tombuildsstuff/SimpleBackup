namespace SimpleBackup.Domain.Engine.Settings
{
    public interface IBackupEngineSettings
    {
        string Password { get; }

        string TempDirectory { get; }
    }
}