namespace SimpleBackup.Domain.Notifiers.Email
{
    using System;
    using System.Net.Mail;

    using SimpleBackup.Domain.Logging;
    using SimpleBackup.Domain.Notifiers.Email.Providers;
    using SimpleBackup.Domain.Notifiers.Email.Settings;

    public class EmailNotifier : IGetNotifiedWhenABackupIsCompleted
	{
        private readonly IEmailSettings _settings;
        private readonly ISmtpProvider _smtpProvider;
		private readonly ILogger _logger;

		public EmailNotifier(IEmailSettings settings, ISmtpProvider smtpProvider, ILogger logger)
		{
		    _settings = settings;
		    _smtpProvider = smtpProvider;
			_logger = logger;
		}

		public string Name
		{
			get
			{
				return "Email";
			}
		}

		public bool Send(bool successful)
		{
			try
			{
                var subject = successful ? _settings.SuccessfulSubject : _settings.FailureSubject;
                var from = string.IsNullOrWhiteSpace(_settings.FromAlias) ? _settings.FromAddress : string.Format("{0} <{1}>", _settings.FromAlias, _settings.FromAddress);
			    var to = string.Join(",", _settings.ToAddresses);
			    var client = _smtpProvider.Get();
                client.Send(new MailMessage(from, to, subject, subject));
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