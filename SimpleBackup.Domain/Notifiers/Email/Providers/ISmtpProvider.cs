namespace SimpleBackup.Domain.Notifiers.Email.Providers
{
    using System.Net.Mail;

    public interface ISmtpProvider
    {
        SmtpClient Get();
    }
}