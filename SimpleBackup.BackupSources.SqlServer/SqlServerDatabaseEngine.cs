namespace SimpleBackup.BackupSources.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using SimpleBackup.Domain.Databases;

    public class SqlServerDatabaseEngine : IDatabaseEngine
    {
	    private readonly string _connectionString;

        public SqlServerDatabaseEngine(string server)
        {
            _connectionString = string.Format("Server={0};Integrated Security=SSPI", server);
        }

        public SqlServerDatabaseEngine(string server, string user, string password)
        {
	        _connectionString = string.Format("Server={0};User ID={1};Password={2}", server, user, password);
        }

	    public string GetName()
        {
            return "Microsoft SQL Server";
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            var systemDatabases = new[] { "master", "model", "msdb", "tempdb" };
            var results = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
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

        public void BackupDatabaseToFile(string databaseName, string fileName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(string.Format("BACKUP DATABASE {0} TO DISK = '{1}' WITH FORMAT, MEDIANAME = '{0}', NAME = '{0}'", databaseName, fileName), connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
				}
            }
        }

        public void RestoreDatabaseFromFile(string databaseName, string filePath, string restoreDirectory)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = string.Format("RESTORE DATABASE {0} FROM DISK='{1}' WITH MOVE '{0}' TO '{2}\\{0}.mdf', MOVE '{0}_log' TO '{2}\\{0}_log.ldf', STATS=5", databaseName, filePath, restoreDirectory);
                using (var command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}