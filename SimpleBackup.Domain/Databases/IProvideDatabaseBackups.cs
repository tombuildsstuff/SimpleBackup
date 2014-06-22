namespace SimpleBackup.Domain.Databases
{
    using System.Collections.Generic;

    public interface IProvideDatabaseBackups
    {
        void BackupDatabaseToFile(string databaseName, string fileName);

        IEnumerable<string> DatabaseNames { get; }

        bool Enabled { get; }

        string GetName { get; }
    }
}