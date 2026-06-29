namespace GreatReports.Infrastructure.Mailer;

public class MailProviderManagerClient(HttpClient httpClient)
{
    public HttpClient HttpClient { get; } = httpClient;
}
