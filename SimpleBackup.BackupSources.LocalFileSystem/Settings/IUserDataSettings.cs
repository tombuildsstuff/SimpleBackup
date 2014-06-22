namespace SimpleBackup.BackupSources.LocalFileSystem.Settings
{
    using System.Collections.Generic;

    using SimpleBackup.Domain.UserDefinedDirectories;

    public interface IUserDataSettings
    {
        bool BackupsEnabled { get; }

        IList<UserDefinedDirectory> DirectoriesToBackup { get; }
    }
}