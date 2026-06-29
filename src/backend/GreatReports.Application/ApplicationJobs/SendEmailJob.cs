using GreatReports.Application.Common.Interfaces;

namespace GreatReports.Application.ApplicationJobs;

public class SendEmailJob(IEmailSender emailSender)
{
    public Task ExecuteAsync(string recipient, string subject, string body, CancellationToken cancellationToken)
    {
        return emailSender.SendEmailAsync(recipient, subject, body, cancellationToken);
    }
}
