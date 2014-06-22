namespace SimpleBackup.Domain.Compression
{
    public interface ICompressData
    {
	    void CompressDataInToFile(string directory, string password, string outputFile);
    }
}