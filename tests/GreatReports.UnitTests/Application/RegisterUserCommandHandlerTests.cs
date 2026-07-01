using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Users.Commands;
using GreatReports.Application.UseCases.Users.CommandHandlers;
using GreatReports.Domain.Entities;
using Moq;

namespace GreatReports.UnitTests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IProviderCompanyRepository> _providerCompanyRepositoryMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object,
            _providerCompanyRepositoryMock.Object,
            _identityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProviderCompanyNotFound()
    {
        // Arrange
        var command = new RegisterUserCommand(Guid.NewGuid(), "João Silva", "test@email.com", "Partner");
        _providerCompanyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProviderCompany?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("ProviderCompany.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExistsWithEmail()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var existingUser = User.Create(provider.Id, "João", "test@email.com").Value;
        var command = new RegisterUserCommand(provider.Id, "João Silva", "test@email.com", "Partner");

        _providerCompanyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.EmailAlreadyExists", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserCreationFails()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var command = new RegisterUserCommand(provider.Id, "", "test@email.com", "Partner"); // Empty name fails creation

        _providerCompanyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.InvalidDisplayName", result.Error.Code);
    }

    [Fact]
    public async Task Handle_ShouldRollbackAndReturnFailure_WhenIdentityServiceFails()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var command = new RegisterUserCommand(provider.Id, "João Silva", "test@email.com", "Partner");

        _providerCompanyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(command.ProviderCompanyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _identityServiceMock
            .Setup(id => id.CreateUserAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(false); // Identity fails!

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User.RegistrationRollback", result.Error.Code);

        // Verify that AddAsync was called, but Delete was also called because of rollback
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenIdentityServiceSucceeds()
    {
        // Arrange
        var provider = ProviderCompany.Create("Provider", "Tax", Guid.CreateVersion7()).Value;
        var command = new RegisterUserCommand(provider.Id, "João Silva", "test@email.com", "Partner");

        _providerCompanyRepositoryMock
            .Setup(repo => repo.GetByIdAsync(provider.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(provider);
        _userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _identityServiceMock
            .Setup(id => id.CreateUserAsync(
                It.IsAny<Guid>(),
                command.Email,
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(r => r.Contains("Partner"))))
            .ReturnsAsync(true); // Identity succeeds!

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        // Verify that AddAsync was called, and Delete was NOT called
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.Delete(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
