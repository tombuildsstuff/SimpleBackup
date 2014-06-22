namespace SimpleBackup
{
    using SimpleBackup.Infrastructure.DI;
	using SimpleBackup.Infrastructure.Runner;

    public class Program
	{
		public static void Main(string[] args)
		{
			var kernel = CastleConfiguration.Build();
		    var runner = kernel.Resolve<IConsoleBackupRunner>();
		    runner.Run();
		}
	}
}