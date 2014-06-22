namespace SimpleBackup.Domain.Notifiers
{
	public interface IGetNotifiedWhenABackupIsCompleted
	{
		string Name { get; }

		bool Send(string file, bool successful);
	}
}