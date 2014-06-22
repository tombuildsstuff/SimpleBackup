namespace SimpleBackup.Domain.Notifiers.Email.Settings
{
    using System.Collections.Generic;
    using System.Configuration;

    public class EmailSettings : IEmailSettings
    {
        public string FromAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.FromAddress"];
            }
        }

        public string FromAlias
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.FromAlias"];
            }
        }

        public string FailureSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.FailureSubject"];
            }
        }

        public string SmtpPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.SmtpPassword"];
            }
        }

        public int SmtpPort
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["Email.SmtpPort"]);
            }
        }

        public bool SmtpRequiresSsl
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["Email.SmtpRequiresSsl"]);
            }
        }

        public string SmtpSenderAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.SmtpSenderAddress"];
            }
        }

        public string SmtpServer
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.SmtpServer"];
            }
        }

        public string SuccessfulSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.SuccessfulSubject"];
            }
        }

        public IEnumerable<string> ToAddresses
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.ToAddresses"].Split(';');
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["Email.UseDefaultCredentials"]);
            }
        }
    }
}