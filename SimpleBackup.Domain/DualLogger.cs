namespace SimpleBackup.Domain
{
	using System;
	using Domain.Interfaces;

	public class DualLogger : ILogger
	{
		private readonly ILogger _logger1;
		private readonly ILogger _logger2;

		public DualLogger(ILogger logger1, ILogger logger2)
		{
			_logger1 = logger1;
			_logger2 = logger2;
		}

		public void Error(string message)
		{
			_logger1.Error(FormatMessage(message));
			_logger2.Error(FormatMessage(message));
		}

		public void Warning(string message)
		{
			_logger1.Warning(FormatMessage(message));
			_logger2.Warning(FormatMessage(message));
		}

		public void Information(string message)
		{
			_logger1.Information(FormatMessage(message));
			_logger2.Information(FormatMessage(message));
		}

		public void ExportToFile(string file)
		{
			_logger1.ExportToFile(file);
			_logger2.ExportToFile(file);
		}

		private static string FormatMessage(string message)
		{
			return string.Format("{0}: {1}", DateTime.Now.ToShortTimeString(), message);
		}
	}
}