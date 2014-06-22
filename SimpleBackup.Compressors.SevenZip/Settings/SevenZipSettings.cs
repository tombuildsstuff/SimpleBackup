namespace SimpleBackup.Compressors.SevenZip.Settings
{
    using System.IO;

    public class SevenZipSettings : ISevenZipSettings
    {
        public string InstallationDirectory
        {
            get
            {
                return null;
            }
        }

        public string SevenZipFileName
        {
            get
            {
                return Path.Combine(InstallationDirectory, "7z.exe");
            }
        }
    }
}