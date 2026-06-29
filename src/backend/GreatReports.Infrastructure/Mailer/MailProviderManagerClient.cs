namespace GreatReports.Infrastructure.Mailer;

public class MailProviderManagerClient
{
    public HttpClient HttpClient { get; }

    public MailProviderManagerClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }
}
