namespace SimpleBackup.BackupSources.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using SimpleBackup.BackupSources.SqlServer.Constants;
    using SimpleBackup.BackupSources.SqlServer.Settings;
    using SimpleBackup.Domain.Databases;

    public class SqlServerBackupSource : IProvideDatabaseBackups
    {
        private readonly ISqlServerSettings _settings;

        public SqlServerBackupSource(ISqlServerSettings settings)
        {
            _settings = settings;
        }

        public void BackupDatabaseToFile(string databaseName, string fileName)
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                using (var command = new SqlCommand(string.Format("BACKUP DATABASE {0} TO DISK = '{1}' WITH FORMAT, MEDIANAME = '{0}', NAME = '{0}'", databaseName, fileName), connection))
                {
                    command.CommandTimeout = _settings.Timeout;
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
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

        public string GetName
        {
            get
            {
                return Names.SqlServerBackupProviderName;
            }
        }
    }
}