namespace SimpleBackup.Compressors.SevenZip.Settings
{
    using System.Configuration;

    public class SevenZipSettings : ISevenZipSettings
    {
        public string SevenZipFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["SevenZip.FilePath"];
            }
        }
    }
}