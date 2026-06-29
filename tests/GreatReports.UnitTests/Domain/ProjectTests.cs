using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class ProjectTests
{
    [Fact]
    public void Create_ShouldReturnFailure_WhenClientCompanyIdIsEmpty()
    {
        // Act
        var result = Project.Create(Guid.Empty, "Projeto Alpha", "Descrição do projeto");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Project.InvalidClientCompany", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnFailure_WhenNameIsEmpty()
    {
        // Act
        var result = Project.Create(Guid.NewGuid(), "", "Descrição do projeto");

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Project.InvalidName", result.Error.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenArgumentsAreValid()
    {
        // Arrange
        var clientCompanyId = Guid.NewGuid();
        var name = "Projeto Alpha";
        var description = "Descrição do projeto";

        // Act
        var result = Project.Create(clientCompanyId, name, description);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(clientCompanyId, result.Value.ClientCompanyId);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(description, result.Value.Description);
    }

    [Fact]
    public void PrivateConstructor_ShouldBeInstantiableViaReflection_ForEFCore()
    {
        // Act
        var instance = (Project)Activator.CreateInstance(typeof(Project), nonPublic: true)!;

        // Assert
        Assert.NotNull(instance);
        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.True(instance.Active);
    }
}
