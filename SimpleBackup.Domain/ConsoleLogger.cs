namespace SimpleBackup.Domain
{
	using System;
	using Domain.Interfaces;

	public class ConsoleLogger : ILogger
	{
		public void Error(string message)
		{
			Console.WriteLine("ERROR: {0}", message);
		}

		public void Warning(string message)
		{
			Console.WriteLine("WARN: {0}", message);
		}

		public void Information(string message)
		{
			Console.WriteLine("INFO: {0}", message);
		}

		public void ExportToFile(string file)
		{
		}
	}
}