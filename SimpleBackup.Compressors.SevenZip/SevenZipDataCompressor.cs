namespace SimpleBackup.Compressors.SevenZip
{
    using System.Diagnostics;
    using System.IO;

    using SimpleBackup.Compressors.SevenZip.Settings;
    using SimpleBackup.Domain.Interfaces;

    public class SevenZipDataCompressor : ICompressData
	{
        private readonly ISevenZipSettings _settings;
        private readonly ILogger _logger;

        public SevenZipDataCompressor(ISevenZipSettings settings, ILogger logger)
		{
            _settings = settings;
            _logger = logger;
		}

		public void CompressDataInToFile(string directory, string password, string outputFile)
		{
			// NOTE: the password has to sit right next to the 'p' bit, otherwise it's not accepted. Yes it's stupid, no I didn't make 7Zip.
			var arguments = string.Format("a -r -p{2} {0} {1}", outputFile, directory, password);

			_logger.Information(string.Format("Checking for the prescence of the output file ({0})", outputFile));
			if (File.Exists(outputFile))
			{
				_logger.Warning("File exists - deleting");
				File.Delete(outputFile);
				_logger.Information("File's deleted - continuing");
			}
			else
			{
				_logger.Information("File does not exist - continuing");
			}

			_logger.Information(string.Format("Launching 7Zip with the command: '{0} {1}'", _settings.SevenZipFileName, arguments));

            var process = new Process { StartInfo = new ProcessStartInfo(_settings.SevenZipFileName, arguments) };
			process.Start();
			process.WaitForExit();

			_logger.Information("7Zip finished");

			// TODO: might be worth outputting the log of 7Zip to a verbose log / seperate file

			_logger.Information(string.Format("Rehecking for the prescence of the output file ({0})", outputFile));
			if (!File.Exists(outputFile))
				throw new FileNotFoundException(string.Format("7Zip had a problem backing up the contents of the directory"));
		}
	}
}