namespace SimpleBackup.Domain.Sources.Databases
{
    public interface IHandleDatabaseRestores
    {
        bool HandlesRestoresFor(string providerName);

        string Name { get; }

        bool RestoreDatabaseFromFile(string databaseName, string filePath, string restoreDirectory);
    }
}