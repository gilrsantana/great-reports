namespace GreatReports.Application.Common.Interfaces;

public interface IEmailVerificationService
{
    Task SendVerificationEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}
