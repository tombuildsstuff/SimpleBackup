﻿namespace SimpleBackup.Infrastructure.Runner
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SimpleBackup.Domain.Engine;
    using SimpleBackup.Domain.Engine.Settings;
    using SimpleBackup.Domain.Logging;
    using SimpleBackup.Domain.Notifiers;

    public class ConsoleBackupRunner : IConsoleBackupRunner
    {
        private readonly IBackupEngine _engine;
        private readonly ILogger _logger;
        private readonly IList<IGetNotifiedWhenABackupIsCompleted> _notifySources;
        private readonly IBackupEngineSettings _settings;

        public ConsoleBackupRunner(IBackupEngine engine, ILogger logger, IList<IGetNotifiedWhenABackupIsCompleted> notifySources, IBackupEngineSettings settings)
        {
            _engine = engine;
            _logger = logger;
            _notifySources = notifySources;
            _settings = settings;
        }

        public void Run()
        {
            var successful = false;

            try
            {
                var tempDirectory = _settings.TempDirectory;

                if (Directory.Exists(tempDirectory))
                {
                    _logger.Information("Temp Directory Exists - Deleting");
                    Directory.Delete(tempDirectory, true);
                }

                _logger.Information("Creating the Temp Directory");
                Directory.CreateDirectory(tempDirectory);

                _logger.Information("Starting Backup");
                successful = _engine.RunBackup();
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Exception Running Backup {0}", ex.Message));
                _logger.Error(ex.StackTrace);
            }

            foreach (var outcomeNotifier in _notifySources)
            {
                _logger.Information(string.Format("Sending log via {0}", outcomeNotifier.Name));
                var outcome = outcomeNotifier.Send(successful);
                _logger.Information(string.Format("Sending {0}", outcome ? "Successful" : "Failed"));
            }
        }
    }
}