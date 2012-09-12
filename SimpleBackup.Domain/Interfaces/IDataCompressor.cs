namespace SimpleBackup.Domain.Interfaces
{
    public interface IDataCompressor
    {
        byte[] CompressData(string directory, string password);
    }
}