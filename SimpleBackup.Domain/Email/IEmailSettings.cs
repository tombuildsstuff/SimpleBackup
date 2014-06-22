namespace SimpleBackup.Domain.Email
{
    using System.Collections.Generic;

    public interface IEmailSettings
    {
        string FromAddress { get; }

        string FromAlias { get; }

        string FailureSubject { get; }

        string SuccessfulSubject { get; }

        IList<string> ToAddresses { get; }
    }
}