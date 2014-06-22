namespace SimpleBackup.Domain.Email
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    using SimpleBackup.Domain.Email.Settings;
    using SimpleBackup.Domain.Interfaces;

    public class EmailNotifier : IGetNotifiedWhenABackupIsCompleted
	{
        private readonly IEmailSettings _settings;

        private readonly SmtpClient _smtpClient;
		private readonly ILogger _logger;

		public EmailNotifier(IEmailSettings settings, SmtpClient smtpClient, ILogger logger)
		{
		    _settings = settings;
		    _smtpClient = smtpClient;
			_logger = logger;
		}

		public string Name
		{
			get
			{
				return "Email";
			}
		}

		public bool Send(string file, bool successful)
		{
			try
			{
				var lines = File.ReadAllLines(file).ToList();
				
				var numberOfErrors = lines.Count(l => l.Contains("ERROR"));
				var numberOfWarnings = lines.Count(l => l.Contains("WARN"));
                var subject = numberOfErrors > 0 || numberOfWarnings > 0 || !successful ? _settings.FailureSubject : _settings.SuccessfulSubject;

				var outcome = new StringBuilder(string.Format("Backup Report for {0} at {1} ({2} errors & {3} warnings)",
												DateTime.Now.ToLongDateString(),
												DateTime.Now.ToShortTimeString(),
												numberOfErrors,
												numberOfWarnings));
				outcome.Append("\n\n");
				foreach (var line in lines)
					outcome.AppendLine(line);

                var from = string.IsNullOrWhiteSpace(_settings.FromAlias) ? _settings.FromAddress : string.Format("{0} <{1}>", _settings.FromAlias, _settings.FromAddress);
			    var to = string.Join(";", _settings.ToAddresses);
                _smtpClient.Send(new MailMessage(from, to, subject, outcome.ToString()));
				return true;
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
				_logger.Error(ex.StackTrace);
				return false;
			}
		}
	}
}