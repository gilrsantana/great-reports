using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public class EmailAuditLog : BaseEntity
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Recipient { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public DateTimeOffset SentAt { get; private set; }
    public bool Success { get; private set; }
    public string? ErrorMessage { get; private set; }

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

        if (!EmailRegex.IsMatch(recipient))
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
