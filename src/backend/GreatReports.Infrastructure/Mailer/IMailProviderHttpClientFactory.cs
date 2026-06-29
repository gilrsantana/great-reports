namespace GreatReports.Infrastructure.Mailer;

public interface IMailProviderHttpClientFactory
{
    MailProviderManagerClient CreateManagerClient();
    MailProviderSenderClient CreateSenderClient();
}
