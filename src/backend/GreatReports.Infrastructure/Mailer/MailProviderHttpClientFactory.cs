using Microsoft.Extensions.DependencyInjection;

namespace GreatReports.Infrastructure.Mailer;

public class MailProviderHttpClientFactory : IMailProviderHttpClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MailProviderHttpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MailProviderManagerClient CreateManagerClient()
    {
        return _serviceProvider.GetRequiredService<MailProviderManagerClient>();
    }

    public MailProviderSenderClient CreateSenderClient()
    {
        return _serviceProvider.GetRequiredService<MailProviderSenderClient>();
    }
}
