namespace SimpleBackup.Domain.Databases
{
    using System.Collections.Generic;

    public interface IDatabaseEngine
    {
        string GetName();

        IEnumerable<string> GetDatabaseNames();

        void BackupDatabaseToFile(string databaseName, string fileName);

        void RestoreDatabaseFromFile(string databaseName, string file, string restoreDirectory);
    }
}