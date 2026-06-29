using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class EmailAuditLogTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenRecipientIsEmpty()
    {
        // Act
        var result = EmailAuditLog.Create("", "Subject", "Body", DateTimeOffset.UtcNow, true, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("EmailAuditLog.RecipientRequired", result.Error.Code);
        Assert.Equal("O destinatário do e-mail é obrigatório.", result.Error.Description);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenRecipientIsInvalid()
    {
        // Act
        var result = EmailAuditLog.Create("invalid-email", "Subject", "Body", DateTimeOffset.UtcNow, true, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("EmailAuditLog.InvalidRecipientFormat", result.Error.Code);
        Assert.Equal("O e-mail do destinatário está em um formato inválido.", result.Error.Description);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenSubjectIsEmpty()
    {
        // Act
        var result = EmailAuditLog.Create("test@example.com", "", "Body", DateTimeOffset.UtcNow, true, null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("EmailAuditLog.SubjectRequired", result.Error.Code);
        Assert.Equal("O assunto do e-mail é obrigatório.", result.Error.Description);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenInputsAreValid()
    {
        // Arrange
        var recipient = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";
        var sentAt = DateTimeOffset.UtcNow;
        var success = true;
        string? errorMessage = null;

        // Act
        var result = EmailAuditLog.Create(recipient, subject, body, sentAt, success, errorMessage);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(recipient, result.Value.Recipient);
        Assert.Equal(subject, result.Value.Subject);
        Assert.Equal(body, result.Value.Body);
        Assert.Equal(sentAt, result.Value.SentAt);
        Assert.Equal(success, result.Value.Success);
        Assert.Equal(errorMessage, result.Value.ErrorMessage);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (EmailAuditLog)Activator.CreateInstance(typeof(EmailAuditLog), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
