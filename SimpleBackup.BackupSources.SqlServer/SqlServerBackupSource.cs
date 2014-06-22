namespace SimpleBackup.BackupSources.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using SimpleBackup.BackupSources.SqlServer.Constants;
    using SimpleBackup.BackupSources.SqlServer.Settings;
    using SimpleBackup.Domain.Databases;
    using SimpleBackup.Domain.Interfaces;

    public class SqlServerBackupSource : IProvideDatabaseBackups
    {
        private readonly ISqlServerSettings _settings;
        private readonly ILogger _logger;

        public SqlServerBackupSource(ISqlServerSettings settings, ILogger logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public bool BackupDatabaseToFile(string databaseName, string fileName)
        {
            _logger.Information(string.Format("Backing up Database '{0}' into '{1}'", databaseName, fileName));
            try
            {
                using (var connection = new SqlConnection(_settings.ConnectionString))
                {
                    using (var command = new SqlCommand(string.Format("BACKUP DATABASE {0} TO DISK = '{1}' WITH FORMAT, MEDIANAME = '{0}', NAME = '{0}'", databaseName, fileName), connection))
                    {
                        command.CommandTimeout = _settings.Timeout;
                        connection.Open();
                        command.ExecuteNonQuery();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return false;
        }

        public IEnumerable<string> DatabaseNames
        {
            get
            {
                var systemDatabases = new[] { "master", "model", "msdb", "tempdb" };
                var results = new List<string>();

                using (var connection = new SqlConnection(_settings.ConnectionString))
                {
                    using (var command = new SqlCommand("EXEC sp_databases", connection))
                    {
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                            results.Add((string)reader["DATABASE_NAME"]);
                    }
                }

                return results.Where(r => !systemDatabases.Any(sd => sd.Equals(r, StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        public bool Enabled
        {
            get
            {
                return _settings.BackupEnabled;
            }
        }

        public string Name
        {
            get
            {
                return Names.SqlServerBackupProviderName;
            }
        }
    }
}