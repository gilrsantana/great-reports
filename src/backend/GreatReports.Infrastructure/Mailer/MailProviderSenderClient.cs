using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using GreatReports.Infrastructure.Configurations;

namespace GreatReports.Infrastructure.Mailer;

public class MailProviderSenderClient(HttpClient httpClient, IOptions<MailProviderSettings> settings)
{
    public HttpClient HttpClient { get; } = httpClient;
    private readonly MailProviderSettings _settings = settings.Value;

    public async Task<HttpResponseMessage> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            from = $"no-reply@{_settings.Domain}",
            to = recipient,
            subject,
            html = body
        };

        return await HttpClient.PostAsJsonAsync("emails", payload, cancellationToken);
    }
}
