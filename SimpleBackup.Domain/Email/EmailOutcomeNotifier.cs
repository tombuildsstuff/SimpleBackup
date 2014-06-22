namespace SimpleBackup.Domain.Email
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;

    using SimpleBackup.Domain.Interfaces;

    public class EmailOutcomeNotifier : IOutcomeNotifier
	{
		private readonly EmailConfiguration _configuration;
		private readonly SmtpClient _smtpClient;
		private readonly ILogger _logger;

		public EmailOutcomeNotifier(EmailConfiguration configuration, SmtpClient smtpClient, ILogger logger)
		{
			_configuration = configuration;
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
				
				var numberOfErrors = lines.Count(l => l.StartsWith("ERROR"));
				var numberOfWarnings = lines.Count(l => l.StartsWith("WARN"));
                var subject = numberOfErrors > 0 || numberOfWarnings > 0 ? _configuration.FailureSubject : _configuration.SuccessfulSubject;

				var outcome = new StringBuilder(string.Format("Backup Report for {0} at {1} ({2} errors & {3} warnings)",
												DateTime.Now.ToLongDateString(),
												DateTime.Now.ToShortTimeString(),
												numberOfErrors,
												numberOfWarnings));
				outcome.Append("\n\n");
				foreach (var line in lines)
					outcome.AppendLine(line);

                _smtpClient.Send(new MailMessage(_configuration.From, _configuration.To, subject, outcome.ToString()));
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