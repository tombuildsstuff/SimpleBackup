namespace SimpleBackup.Domain.Interfaces
{
	public interface IOutcomeNotifier
	{
		string Name { get; }

		bool Send(string file);
	}
}