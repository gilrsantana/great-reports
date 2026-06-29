using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public sealed partial class EmailAuditLog : BaseEntity
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public string Recipient { get; } = string.Empty;
    public string Subject { get; } = string.Empty;
    public string Body { get; } = string.Empty;
    public DateTimeOffset SentAt { get; }
    public bool Success { get; }
    public string? ErrorMessage { get; }

    // EF Core constructor
    private EmailAuditLog() : base()
    {
    }

    private EmailAuditLog(string recipient, string subject, string body, DateTimeOffset sentAt, bool success, string? errorMessage) : base()
    {
        Recipient = recipient;
        Subject = subject;
        Body = body;
        SentAt = sentAt;
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static Result<EmailAuditLog> Create(string recipient, string subject, string body, DateTimeOffset sentAt, bool success, string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(recipient))
        {
            return Result.Failure<EmailAuditLog>(new Error("EmailAuditLog.RecipientRequired", "O destinatário do e-mail é obrigatório."));
        }

        if (!EmailRegex().IsMatch(recipient))
        {
            return Result.Failure<EmailAuditLog>(new Error("EmailAuditLog.InvalidRecipientFormat", "O e-mail do destinatário está em um formato inválido."));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            return Result.Failure<EmailAuditLog>(new Error("EmailAuditLog.SubjectRequired", "O assunto do e-mail é obrigatório."));
        }

        return new EmailAuditLog(recipient, subject, body ?? string.Empty, sentAt, success, errorMessage);
    }
}
