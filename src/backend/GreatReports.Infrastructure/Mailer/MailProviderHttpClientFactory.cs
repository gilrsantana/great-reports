using Microsoft.Extensions.DependencyInjection;

namespace GreatReports.Infrastructure.Mailer;

public class MailProviderHttpClientFactory(IServiceProvider serviceProvider) : IMailProviderHttpClientFactory
{
    public MailProviderManagerClient CreateManagerClient()
    {
        return serviceProvider.GetRequiredService<MailProviderManagerClient>();
    }

    public MailProviderSenderClient CreateSenderClient()
    {
        return serviceProvider.GetRequiredService<MailProviderSenderClient>();
    }
}
