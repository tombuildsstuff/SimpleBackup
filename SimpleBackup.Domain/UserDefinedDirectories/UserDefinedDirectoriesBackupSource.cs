namespace SimpleBackup.Domain.UserDefinedDirectories
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using SimpleBackup.Domain.Interfaces;

    public class UserDefinedDirectoriesBackupSource : IBackupSource
    {
        private readonly IEnumerable<UserDefinedDirectory> _directories;

        public UserDefinedDirectoriesBackupSource(IEnumerable<UserDefinedDirectory> directories)
        {
            _directories = directories;
        }

        public void BackupIntoDirectory(string directory)
        {
            foreach (var directoryToBackup in _directories)
            {
                var directoryPath = string.Concat(directory, Path.DirectorySeparatorChar.ToString(), directoryToBackup.FriendlyName, Path.DirectorySeparatorChar.ToString());
                var childDirectories = Directory.EnumerateDirectories(directoryToBackup.Path, "*", SearchOption.AllDirectories);
                foreach (var childDirectory in childDirectories)
                    Directory.CreateDirectory(childDirectory.Replace(directoryToBackup.Path, directoryPath));

                var files = Directory.EnumerateFiles(directoryToBackup.Path, "*.*", SearchOption.AllDirectories).ToList();
                foreach (var file in files)
                    File.Copy(file, file.Replace(directoryToBackup.Path, directoryPath), false);
            }
        }

        public string Name
        {
            get
            {
                return "User Defined Directories";
            }
        }
    }
}