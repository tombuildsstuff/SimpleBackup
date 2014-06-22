namespace SimpleBackup.Compressors.SevenZip.Settings
{
    public interface ISevenZipSettings
    {
        string InstallationDirectory { get; }

        string SevenZipFileName { get; }
    }
}