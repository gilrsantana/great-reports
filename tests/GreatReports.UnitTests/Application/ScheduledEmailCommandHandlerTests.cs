using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ScheduledEmails.Commands;
using GreatReports.Application.UseCases.ScheduledEmails.CommandHandlers;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;
using Moq;

namespace GreatReports.UnitTests.Application;

public class ScheduledEmailCommandHandlerTests
{
    private readonly Mock<IScheduledEmailRepository> _scheduledEmailRepositoryMock = new();
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IClientContactRepository> _clientContactRepositoryMock = new();
    private readonly Mock<IScheduledEmailReceiverRepository> _receiverRepositoryMock = new();

    private readonly CreateScheduledEmailCommandHandler _createHandler;
    private readonly AddScheduledEmailReceiverCommandHandler _addReceiverHandler;

    public ScheduledEmailCommandHandlerTests()
    {
        _createHandler = new CreateScheduledEmailCommandHandler(
            _scheduledEmailRepositoryMock.Object,
            _groupRepositoryMock.Object);

        _addReceiverHandler = new AddScheduledEmailReceiverCommandHandler(
            _receiverRepositoryMock.Object,
            _scheduledEmailRepositoryMock.Object,
            _userRepositoryMock.Object,
            _clientContactRepositoryMock.Object);
    }

    [Fact]
    public async Task Create_ShouldSucceed_WhenParametersAreValid()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var group = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Test Group", "UTC").Value;

        _groupRepositoryMock.Setup(r => r.GetByIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var command = new CreateScheduledEmailCommand(
            groupId,
            "Agendamento Diário",
            "0 8 * * *",
            ReportFrequency.Daily,
            null);

        // Act
        var result = await _createHandler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        _scheduledEmailRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ScheduledEmail>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldFail_WhenGroupNotFound()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        _groupRepositoryMock.Setup(r => r.GetByIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var command = new CreateScheduledEmailCommand(
            groupId,
            "Agendamento Diário",
            "0 8 * * *",
            ReportFrequency.Daily,
            null);

        // Act
        var result = await _createHandler.HandleAsync(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task AddReceiver_ShouldSucceed_WhenParametersAreValid()
    {
        // Arrange
        var scheduleId = Guid.NewGuid();
        var schedule = ScheduledEmail.Create(Guid.NewGuid(), "Agendamento", "0 8 * * *", ReportFrequency.Daily).Value;
        
        var userId = Guid.NewGuid();
        var user = User.Create(
            Guid.NewGuid(),
            "Destinatário",
            "dest@test.com"
        ).Value;

        _scheduledEmailRepositoryMock.Setup(r => r.GetByIdAsync(scheduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(schedule);

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var command = new AddScheduledEmailReceiverCommand(scheduleId, userId, null);

        // Act
        var result = await _addReceiverHandler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        _receiverRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ScheduledEmailReceiver>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
