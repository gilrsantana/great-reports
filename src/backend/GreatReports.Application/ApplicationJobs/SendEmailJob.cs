using GreatReports.Application.Common.Interfaces;

namespace GreatReports.Application.ApplicationJobs;

public class SendEmailJob
{
    private readonly IEmailSender _emailSender;

    public SendEmailJob(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task ExecuteAsync(string recipient, string subject, string body, CancellationToken cancellationToken)
    {
        await _emailSender.SendEmailAsync(recipient, subject, body, cancellationToken);
    }
}
