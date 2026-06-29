using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Groups.Commands;
using GreatReports.Application.UseCases.Groups.CommandHandlers;
using GreatReports.Domain.Entities;
using Moq;

namespace GreatReports.UnitTests.Application;

public class CreateGroupCommandHandlerTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly CreateGroupCommandHandler _handler;

    public CreateGroupCommandHandlerTests()
    {
        _handler = new CreateGroupCommandHandler(
            _groupRepositoryMock.Object,
            _userRepositoryMock.Object,
            _clientCompanyRepositoryMock.Object,
            _projectRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenGroupLeaderNotFound()
    {
        // Arrange
        var command = new CreateGroupCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Grupo Alpha", "America/Sao_Paulo");
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupLeaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.GroupLeaderNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClientCompanyNotFound()
    {
        // Arrange
        var leader = User.Create(Guid.NewGuid(), "Leader", "leader@email.com").Value;
        var command = new CreateGroupCommand(leader.Id, Guid.NewGuid(), Guid.NewGuid(), "Grupo Alpha", "America/Sao_Paulo");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupLeaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leader);
        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientCompany?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ClientCompany.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProjectNotFound()
    {
        // Arrange
        var leader = User.Create(Guid.NewGuid(), "Leader", "leader@email.com").Value;
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new CreateGroupCommand(leader.Id, clientCompany.Id, Guid.NewGuid(), "Grupo Alpha", "America/Sao_Paulo");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupLeaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leader);
        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);
        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Project.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenGroupCreationFails()
    {
        // Arrange
        var leader = User.Create(Guid.NewGuid(), "Leader", "leader@email.com").Value;
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var project = Project.Create(clientCompany.Id, "Project", "Desc").Value;
        var command = new CreateGroupCommand(leader.Id, clientCompany.Id, project.Id, "Grupo Alpha", "Invalid/Timezone");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupLeaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leader);
        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);
        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.TimezoneNotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var leader = User.Create(Guid.NewGuid(), "Leader", "leader@email.com").Value;
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var project = Project.Create(clientCompany.Id, "Project", "Desc").Value;
        var command = new CreateGroupCommand(leader.Id, clientCompany.Id, project.Id, "Grupo Alpha", "America/Sao_Paulo");

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.GroupLeaderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(leader);
        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);
        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ProjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);
        _groupRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Group>(), It.IsAny<CancellationToken>()), Times.Once);
        _groupRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
