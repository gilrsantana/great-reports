using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using GreatReports.Infrastructure.Configurations;

namespace GreatReports.Infrastructure.Mailer;

public class MailProviderSenderClient
{
    public HttpClient HttpClient { get; }
    private readonly MailProviderSettings _settings;

    public MailProviderSenderClient(HttpClient httpClient, IOptions<MailProviderSettings> settings)
    {
        HttpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<HttpResponseMessage> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            from = $"no-reply@{_settings.Domain}",
            to = recipient,
            subject = subject,
            html = body
        };

        return await HttpClient.PostAsJsonAsync("emails", payload, cancellationToken);
    }
}
