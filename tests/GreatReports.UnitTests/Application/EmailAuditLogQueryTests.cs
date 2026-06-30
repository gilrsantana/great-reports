using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.EmailAuditLogs.Queries;
using GreatReports.Application.UseCases.EmailAuditLogs.QueryHandlers;
using GreatReports.Domain.Entities;
using GreatReports.Shared;
using Moq;

namespace GreatReports.UnitTests.Application;

public class EmailAuditLogQueryTests
{
    private readonly Mock<IEmailAuditLogRepository> _repositoryMock = new();
    private readonly GetEmailAuditLogsQueryHandler _handler;

    public EmailAuditLogQueryTests()
    {
        _handler = new GetEmailAuditLogsQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedDtos_WhenQueryIsValid()
    {
        // Arrange
        var logs = new List<EmailAuditLog>
        {
            EmailAuditLog.Create("dest@test.com", "Assunto", "Corpo", DateTimeOffset.UtcNow, true, null).Value
        };

        var pagedResponse = new PagedResponse<EmailAuditLog>(logs, 1, 10, 1);

        _repositoryMock.Setup(r => r.GetPagedLogsAsync(
                It.IsAny<string?>(),
                It.IsAny<bool?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        var query = new GetEmailAuditLogsQuery("dest@test.com", true, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Items);
        Assert.Equal("dest@test.com", result.Value.Items[0].Recipient);
    }
}
