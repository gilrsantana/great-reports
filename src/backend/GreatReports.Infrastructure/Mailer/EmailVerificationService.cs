using GreatReports.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GreatReports.Infrastructure.Mailer;

public class EmailVerificationService : IEmailVerificationService
{
    private readonly ILogger<EmailVerificationService> _logger;

    public EmailVerificationService(ILogger<EmailVerificationService> logger)
    {
        _logger = logger;
    }

    public Task SendVerificationEmailAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        // verificationLink uses the BR-Portuguese frontend path
        var verificationLink = $"http://localhost:4200/confirm-email?email={email}&token={token}";
        
        _logger.LogInformation("================================================");
        _logger.LogInformation("[EMAIL MOCK] Enviando e-mail de confirmação para {Email}", email);
        _logger.LogInformation("Link de confirmação: {Link}", verificationLink);
        _logger.LogInformation("================================================");
        
        return Task.CompletedTask;
    }
}
