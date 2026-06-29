using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Infrastructure.Mailer;

public class MailProviderEmailSender : IEmailSender
{
    private readonly IMailProviderHttpClientFactory _clientFactory;
    private readonly IEmailAuditLogRepository _auditLogRepository;

    public MailProviderEmailSender(
        IMailProviderHttpClientFactory clientFactory,
        IEmailAuditLogRepository auditLogRepository)
    {
        _clientFactory = clientFactory;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<Result> SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        // Validate invariants first by attempting to create a valid EmailAuditLog entity structure
        var validationResult = EmailAuditLog.Create(recipient, subject, body, DateTimeOffset.UtcNow, true, null);
        if (validationResult.IsFailure)
        {
            return Result.Failure(validationResult.Error);
        }

        var sentAt = DateTimeOffset.UtcNow;
        bool success = false;
        string? errorMessage = null;

        try
        {
            var senderClient = _clientFactory.CreateSenderClient();
            var response = await senderClient.SendEmailAsync(recipient, subject, body, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                success = true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                errorMessage = $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {errorContent}";
            }
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }

        // Create the actual audit log with the real outcome
        var auditLog = EmailAuditLog.Create(recipient, subject, body, sentAt, success, errorMessage).Value;

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
        await _auditLogRepository.SaveChangesAsync(cancellationToken);

        if (!success)
        {
            return Result.Failure(new Error("EmailSender.TransmissionFailed", $"Falha ao enviar e-mail para {recipient}: {errorMessage}"));
        }

        return Result.Success();
    }
}
