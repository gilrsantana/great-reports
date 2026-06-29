using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class GroupTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenGroupLeaderIdIsEmpty()
    {
        // Act
        var result = Group.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), "Grupo Alpha", "America/Sao_Paulo");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.InvalidGroupLeader", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenClientCompanyIdIsEmpty()
    {
        // Act
        var result = Group.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Grupo Alpha", "America/Sao_Paulo");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.InvalidClientCompany", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenProjectIdIsEmpty()
    {
        // Act
        var result = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "Grupo Alpha", "America/Sao_Paulo");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.InvalidProject", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "America/Sao_Paulo");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTimezoneIsEmpty()
    {
        // Act
        var result = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Grupo Alpha", "");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.InvalidTimezone", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenTimezoneIsInvalid()
    {
        // Act
        var result = Group.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Grupo Alpha", "Invalid/Timezone_Name");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Group.TimezoneNotFound", result.Error.Code);
        Assert.Equal("O fuso horário fornecido é inválido ou não foi encontrado.", result.Error.Description);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenTimezoneIsValid()
    {
        // Arrange
        var leaderId = Guid.NewGuid();
        var clientCompanyId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var name = "Grupo Alpha";
        var timezone = "America/Sao_Paulo";

        // Act
        var result = Group.Create(leaderId, clientCompanyId, projectId, name, timezone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(leaderId, result.Value.GroupLeaderId);
        Assert.Equal(clientCompanyId, result.Value.ClientCompanyId);
        Assert.Equal(projectId, result.Value.ProjectId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(timezone, result.Value.Timezone);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (Group)Activator.CreateInstance(typeof(Group), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
