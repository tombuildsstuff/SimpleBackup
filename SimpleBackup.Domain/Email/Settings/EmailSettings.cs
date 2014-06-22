namespace SimpleBackup.Domain.Email.Settings
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

        public string SuccessfulSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.SuccessfulSubject"];
            }
        }

        public IList<string> ToAddresses
        {
            get
            {
                return ConfigurationManager.AppSettings["Email.ToAddresses"].Split(';');
            }
        }
    }
}