namespace SimpleBackup.Domain.Notifiers.Email.Providers
{
    using System.Net;
    using System.Net.Mail;

    using SimpleBackup.Domain.Notifiers.Email.Settings;

    public class SmtpProvider : ISmtpProvider
    {
        private readonly IEmailSettings _settings;

        public SmtpProvider(IEmailSettings settings)
        {
            _settings = settings;
        }

        public SmtpClient Get()
        {
            var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                UseDefaultCredentials = _settings.UseDefaultCredentials,
                EnableSsl = _settings.SmtpRequiresSsl
            };

            if (!client.UseDefaultCredentials)
                client.Credentials = new NetworkCredential(_settings.SmtpSenderAddress, _settings.SmtpPassword);

            return client;
        }
    }
}