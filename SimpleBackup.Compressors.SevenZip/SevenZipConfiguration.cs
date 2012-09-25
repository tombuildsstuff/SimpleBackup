namespace SimpleBackup.Compressors.SevenZip
{
	using System.IO;

	public class SevenZipConfiguration
	{
		public string InstallationDirectory { get; set; }

		public string SevenZipFileName
		{
			get { return Path.Combine(InstallationDirectory, "7z.exe"); }
		}

		public SevenZipConfiguration(string installationDirectory)
		{
			InstallationDirectory = installationDirectory;
		}
	}
}