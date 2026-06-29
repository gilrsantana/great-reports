using System.Net;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Infrastructure.Configurations;
using GreatReports.Infrastructure.Mailer;
using GreatReports.Shared;

namespace GreatReports.UnitTests.Infrastructure;

public class MailProviderEmailSenderTests
{
    private readonly Mock<IMailProviderHttpClientFactory> _clientFactoryMock = new();
    private readonly Mock<IEmailAuditLogRepository> _auditLogRepositoryMock = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new(MockBehavior.Strict);
    private readonly MailProviderSettings _settings;
    private readonly MailProviderEmailSender _emailSender;

    public MailProviderEmailSenderTests()
    {
        _settings = new MailProviderSettings
        {
            BaseAddress = "https://api.resend.com/",
            Domain = "testdomain.com",
            SenderApiKey = "re_testkey123"
        };
        var options = Options.Create(_settings);

        // Setup HttpClient with mocked handler
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri(_settings.BaseAddress)
        };

        var senderClient = new MailProviderSenderClient(httpClient, options);
        _clientFactoryMock.Setup(x => x.CreateSenderClient()).Returns(senderClient);

        _emailSender = new MailProviderEmailSender(_clientFactoryMock.Object, _auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnSuccess_WhenHttpCallSucceeds()
    {
        // Arrange
        const string recipient = "recipient@example.com";
        const string subject = "Hello World";
        const string body = "This is a test email body";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"id\":\"123\"}")
            });

        _auditLogRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EmailAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _auditLogRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _emailSender.SendEmailAsync(recipient, subject, body);

        // Assert
        Assert.True(result.IsSuccess);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(It.Is<EmailAuditLog>(log =>
                log.Recipient == recipient &&
                log.Subject == subject &&
                log.Body == body &&
                log.Success &&
                log.ErrorMessage == null),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnFailure_WhenHttpCallFails()
    {
        // Arrange
        const string recipient = "recipient@example.com";
        const string subject = "Hello World";
        const string body = "This is a test email body";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("Invalid API key")
            });

        _auditLogRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EmailAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _auditLogRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _emailSender.SendEmailAsync(recipient, subject, body);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Falha ao enviar e-mail para", result.Error.Description);
        Assert.Contains("HTTP 400", result.Error.Description);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(It.Is<EmailAuditLog>(log =>
                log.Recipient == recipient &&
                log.Subject == subject &&
                log.Body == body &&
                !log.Success &&
                log.ErrorMessage != null),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnFailure_WhenRecipientIsInvalid()
    {
        // Arrange
        const string recipient = "invalid-email";
        const string subject = "Hello World";
        const string body = "This is a test email body";

        // Act
        var result = await _emailSender.SendEmailAsync(recipient, subject, body);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("EmailAuditLog.InvalidRecipientFormat", result.Error.Code);

        // Http and Repository shouldn't be called since domain validation failed first
        _auditLogRepositoryMock.Verify(x => x.AddAsync(It.IsAny<EmailAuditLog>(), It.IsAny<CancellationToken>()), Times.Never);
        _auditLogRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldLogFailure_WhenHttpCallThrowsException()
    {
        // Arrange
        const string recipient = "recipient@example.com";
        const string subject = "Hello World";
        const string body = "This is a test email body";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Throws(new HttpRequestException("Network failure"));

        _auditLogRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<EmailAuditLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _auditLogRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _emailSender.SendEmailSenderAsync(recipient, subject, body);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Network failure", result.Error.Description);

        _auditLogRepositoryMock.Verify(
            x => x.AddAsync(It.Is<EmailAuditLog>(log =>
                log.Recipient == recipient &&
                log.Subject == subject &&
                log.Body == body &&
                !log.Success &&
                log.ErrorMessage == "Network failure"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _auditLogRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

// Extension to map SendEmailAsync to simplify name matching
public static class EmailSenderExtensions
{
    public static Task<Result> SendEmailSenderAsync(this IEmailSender sender, string recipient, string subject, string body)
    {
        return sender.SendEmailAsync(recipient, subject, body);
    }
}
