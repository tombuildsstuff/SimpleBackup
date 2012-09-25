namespace SimpleBackup.Domain.Interfaces
{
    public interface IDataCompressor
    {
	    void CompressDataInToFile(string directory, string password, string outputFile);
    }
}