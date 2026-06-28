using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Projects.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GreatReports.UnitTests.Application;

public class RegisterProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IClientCompanyRepository> _clientCompanyRepositoryMock = new();
    private readonly RegisterProjectCommandHandler _handler;

    public RegisterProjectCommandHandlerTests()
    {
        _handler = new RegisterProjectCommandHandler(
            _projectRepositoryMock.Object,
            _clientCompanyRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenClientCompanyNotFound()
    {
        // Arrange
        var command = new RegisterProjectCommand(Guid.NewGuid(), "Projeto Alpha", "Descrição");
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
    public async Task Handle_ShouldReturnFailure_WhenProjectCreationFails()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new RegisterProjectCommand(clientCompany.Id, "", "Descrição"); // Empty name fails creation

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Project.InvalidName", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
    {
        // Arrange
        var clientCompany = ClientCompany.Create(Guid.NewGuid(), "Client").Value;
        var command = new RegisterProjectCommand(clientCompany.Id, "Projeto Alpha", "Descrição");

        _clientCompanyRepositoryMock
            .Setup(x => x.GetByIdAsync(command.ClientCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(clientCompany);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        _projectRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Once);
        _projectRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
