namespace SimpleBackup.Domain.Interfaces
{
    public interface ICompressData
    {
	    void CompressDataInToFile(string directory, string password, string outputFile);
    }
}