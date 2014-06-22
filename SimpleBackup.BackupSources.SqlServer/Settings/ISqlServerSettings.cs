namespace SimpleBackup.BackupSources.SqlServer.Settings
{
    public interface ISqlServerSettings
    {
        SqlServerAuthType Authentication { get; }

        bool BackupEnabled { get; }

        string ConnectionString { get; }

        string Instance { get; }

        string Password { get; }

        int Timeout { get; }

        string Username { get; }
    }
}