namespace SimpleBackup.Domain.Notifiers
{
	public interface IGetNotifiedWhenABackupIsCompleted
	{
		string Name { get; }

		bool Send(bool successful);
	}
}