namespace SimpleBackup.Domain.Notifiers.Email.Settings
{
    using System.Collections.Generic;

    public interface IEmailSettings
    {
        string FromAddress { get; }

        string FromAlias { get; }

        string FailureSubject { get; }

        int SmtpPort { get; }

        bool SmtpRequiresSsl { get; }

        string SmtpServer { get; }

        string SmtpSenderAddress { get; }

        string SmtpPassword { get; }

        string SuccessfulSubject { get; }

        IEnumerable<string> ToAddresses { get; }

        bool UseDefaultCredentials { get; }
    }
}