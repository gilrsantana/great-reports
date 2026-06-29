using GreatReports.Shared;

namespace GreatReports.Application.Common.Interfaces;

public interface IEmailSender
{
    Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);
}
