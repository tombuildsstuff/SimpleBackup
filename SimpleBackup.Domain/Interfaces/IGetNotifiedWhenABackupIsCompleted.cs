namespace SimpleBackup.Domain.Interfaces
{
	public interface IGetNotifiedWhenABackupIsCompleted
	{
		string Name { get; }

		bool Send(string file, bool successful);
	}
}