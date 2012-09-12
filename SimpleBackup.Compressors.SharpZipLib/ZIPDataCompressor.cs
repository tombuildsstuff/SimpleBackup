namespace SimpleBackup.Compressors.SharpZipLib
{
    using System.IO;

    using ICSharpCode.SharpZipLib.Zip;

    using SimpleBackup.Domain.Interfaces;

    public class ZIPDataCompressor : IDataCompressor
    {
        public byte[] CompressData(string directory, string password)
        {
            var fullFileListing = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories);
            var directories = Directory.EnumerateDirectories(directory, "*", SearchOption.AllDirectories);
            byte[] bytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var zip = new ZipFile(memoryStream))
                {
                    zip.BeginUpdate();

                    foreach (var childDirectory in directories)
                        zip.AddDirectory(childDirectory.Replace(directory, string.Empty));

                    foreach (var file in fullFileListing)
                        zip.Add(file, file.Replace(directory, string.Empty));
                    
                    zip.Password = password;
                    zip.CommitUpdate();
                }

                bytes = memoryStream.ToArray();
            }

            return bytes;
        }
    }
}