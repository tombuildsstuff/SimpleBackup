namespace SimpleBackup.BackupSources.SqlServer
{
    using System;
    using System.Data.SqlClient;

    using SimpleBackup.BackupSources.SqlServer.Constants;
    using SimpleBackup.BackupSources.SqlServer.Settings;
    using SimpleBackup.Domain.Databases;
    using SimpleBackup.Domain.Interfaces;

    public class SqlServerRestoreSource : IHandleDatabaseRestores
    {
        private readonly ISqlServerSettings _settings;
        private readonly ILogger _logger;

        public SqlServerRestoreSource(ISqlServerSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public bool HandlesRestoresFor(string providerName)
        {
            return Name.Equals(providerName, StringComparison.InvariantCultureIgnoreCase);
        }

        public string Name
        {
            get
            {
                return Names.SqlServerBackupProviderName;
            }
        }

        public bool RestoreDatabaseFromFile(string databaseName, string filePath, string restoreDirectory)
        {
            try
            {
                using (var connection = new SqlConnection(_settings.ConnectionString))
                {
                    var sql = string.Format("RESTORE DATABASE {0} FROM DISK='{1}' WITH MOVE '{0}' TO '{2}\\{0}.mdf', MOVE '{0}_log' TO '{2}\\{0}_log.ldf', STATS=5", databaseName, filePath, restoreDirectory);
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = _settings.Timeout;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Unable to restore database '{0}' from '{1}' into {2} using the '{3}' provider", databaseName, filePath, restoreDirectory, Name));
                _logger.Error(ex.ToString());
            }

            return false;
        }
    }
}