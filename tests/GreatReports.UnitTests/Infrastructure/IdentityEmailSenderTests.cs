using System.Linq.Expressions;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.ApplicationJobs;
using GreatReports.Infrastructure.Identity;
using GreatReports.Infrastructure.Mailer;
using Moq;

namespace GreatReports.UnitTests.Infrastructure;

public class IdentityEmailSenderTests
{
    private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock = new();
    private readonly IdentityEmailSender _emailSender;

    public IdentityEmailSenderTests()
    {
        _emailSender = new IdentityEmailSender(_backgroundJobServiceMock.Object);
    }

    [Fact]
    public async Task SendConfirmationLinkAsync_ShouldEnqueueSendEmailJob()
    {
        // Arrange
        var user = new Account { Email = "user@example.com" };
        var email = "user@example.com";
        var link = "https://example.com/confirm";

        // Act
        await _emailSender.SendConfirmationLinkAsync(user, email, link);

        // Assert
        _backgroundJobServiceMock.Verify(
            x => x.Enqueue(It.Is<Expression<Func<SendEmailJob, Task>>>(expr => 
                EvaluateExpression(expr, email, "Confirme seu endereço de e-mail", link))),
            Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetLinkAsync_ShouldEnqueueSendEmailJob()
    {
        // Arrange
        var user = new Account { Email = "user@example.com" };
        var email = "user@example.com";
        var link = "https://example.com/reset";

        // Act
        await _emailSender.SendPasswordResetLinkAsync(user, email, link);

        // Assert
        _backgroundJobServiceMock.Verify(
            x => x.Enqueue(It.Is<Expression<Func<SendEmailJob, Task>>>(expr => 
                EvaluateExpression(expr, email, "Recuperação de senha", link))),
            Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_ShouldEnqueueSendEmailJob()
    {
        // Arrange
        var user = new Account { Email = "user@example.com" };
        var email = "user@example.com";
        var code = "123456";

        // Act
        await _emailSender.SendPasswordResetCodeAsync(user, email, code);

        // Assert
        _backgroundJobServiceMock.Verify(
            x => x.Enqueue(It.Is<Expression<Func<SendEmailJob, Task>>>(expr => 
                EvaluateExpression(expr, email, "Código para recuperação de senha", code))),
            Times.Once);
    }

    private static bool EvaluateExpression(
        Expression<Func<SendEmailJob, Task>> expr, 
        string expectedRecipient, 
        string expectedSubject, 
        string expectedContent)
    {
        if (expr.Body is not MethodCallExpression methodCall) return false;

        var recipient = Expression.Lambda<Func<string>>(methodCall.Arguments[0]).Compile()();
        var subject = Expression.Lambda<Func<string>>(methodCall.Arguments[1]).Compile()();
        var body = Expression.Lambda<Func<string>>(methodCall.Arguments[2]).Compile()();

        return recipient == expectedRecipient && 
               subject == expectedSubject && 
               body.Contains(expectedContent);
    }
}
