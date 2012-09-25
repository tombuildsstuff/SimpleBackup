namespace SimpleBackup.Compressors.SharpZipLib
{
	using System.IO;

	using ICSharpCode.SharpZipLib.Zip;

	using SimpleBackup.Domain.Interfaces;

	public class ZIPDataCompressor : IDataCompressor
	{
		private readonly ILogger _logger;

		public ZIPDataCompressor(ILogger logger)
		{
			_logger = logger;
		}

		public void CompressDataInToFile(string directory, string password, string outputFile)
		{
			var fullFileListing = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);
			var directories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);

			_logger.Information("Creating ZIP File");
			using (var zip = new ZipFile(outputFile))
			{
				zip.UseZip64 = UseZip64.On;

				_logger.Information("Adding directories..");
				foreach (var childDirectory in directories)
				{
					_logger.Information(string.Format("Adding {0}", childDirectory.Replace(directory, string.Empty)));
					zip.BeginUpdate();
					zip.AddDirectory(childDirectory.Replace(directory, string.Empty));
					zip.CommitUpdate();
				}

				_logger.Information("Adding files..");
				foreach (var file in fullFileListing)
				{
					_logger.Information(string.Format("Adding {0}", file.Replace(directory, string.Empty)));
					zip.BeginUpdate();
					zip.Add(file, file.Replace(directory, string.Empty));
					zip.CommitUpdate();
				}

				_logger.Information("Setting password..");
				zip.BeginUpdate();
				zip.Password = password;
				zip.CommitUpdate();
			}
		}
	}
}