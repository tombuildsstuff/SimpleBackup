namespace SimpleBackup.Domain.Databases
{
    using System.Collections.Generic;

    using SimpleBackup.Domain.Interfaces;

    public class DatabaseBackupSource : IBackupSource
    {
        private readonly IEnumerable<IProvideDatabaseBackups> _databaseEngines;

        public DatabaseBackupSource(IEnumerable<IProvideDatabaseBackups> databaseEngines)
        {
            _databaseEngines = databaseEngines;
        }

        public void BackupIntoDirectory(string directory)
        {
            foreach (var engine in _databaseEngines)
            {
                foreach (var db in engine.DatabaseNames)
                    engine.BackupDatabaseToFile(db, string.Format("{0}\\backup-{1}.bak", directory, db));
            }
        }

        public string Name
        {
            get
            {
                return "Databases";
            }
        }
    }
}