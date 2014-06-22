namespace SimpleBackup.Domain.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MultiLogger : ILogger
	{
	    private readonly IList<ILogger> _loggers;

		public MultiLogger(IList<ILogger> loggers)
		{
		    _loggers = loggers;
		}

	    public void Error(string message)
	    {
	        _loggers.ToList().ForEach(l => l.Error(FormatMessage(message)));
	    }

		public void Warning(string message)
        {
            _loggers.ToList().ForEach(l => l.Warning(FormatMessage(message)));
		}

		public void Information(string message)
        {
            _loggers.ToList().ForEach(l => l.Information(FormatMessage(message)));
		}

		private static string FormatMessage(string message)
		{
			return string.Format("{0}: {1}", DateTime.Now.ToShortTimeString(), message);
		}
	}
}