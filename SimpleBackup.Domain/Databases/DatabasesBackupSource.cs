namespace SimpleBackup.Domain.Databases
{
    using System;
    using System.Collections.Generic;

    using SimpleBackup.Domain.Interfaces;

    public class DatabasesBackupSource : IBackupSource
    {
        private readonly IEnumerable<IProvideDatabaseBackups> _databaseEngines;
        private readonly ILogger _logger;

        public DatabasesBackupSource(IEnumerable<IProvideDatabaseBackups> databaseEngines, ILogger logger)
        {
            _databaseEngines = databaseEngines;
            _logger = logger;
        }

        public void BackupIntoDirectory(string directory)
        {
            foreach (var engine in _databaseEngines)
            {
                _logger.Information(string.Format("Current Database Backup Source: {0}", engine.Name));

                if (!engine.Enabled)
                {
                    _logger.Information(string.Format("Database {0} not configured for backup. Skipping..", engine.Name));
                    return;
                }

                foreach (var db in engine.DatabaseNames)
                {
                    _logger.Information(string.Format("Starting backup up DB '{0}' / Provider '{1}'", db, engine.Name));

                    var success = false;
                    var outputFileName = string.Format("{0}\\backup-{1}.bak", directory, db);
                    try
                    {
                        success = engine.BackupDatabaseToFile(db, outputFileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(string.Format("Error backing up DB '{0}' into '{1}' using Provider '{2}'", db, outputFileName, engine.Name));
                        _logger.Error(ex.ToString());
                    }

                    _logger.Information(string.Format("Ended backup up DB '{0}' using Provider '{1}' - status: '{2}'", db, engine.Name, success));
                }
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