namespace SimpleBackup.BackupSources.LocalFileSystem.Settings
{
    using System.Collections.Generic;

    using SimpleBackup.BackupSources.LocalFileSystem.Entities;

    public interface IUserDataSettings
    {
        bool BackupsEnabled { get; }

        IEnumerable<UserDataDirectory> DirectoriesToBackup { get; }
    }
}