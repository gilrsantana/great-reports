using GreatReports.Domain.Entities;

namespace GreatReports.UnitTests.Domain;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        // Concrete subclass of BaseEntity for testing purposes
    }

    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True((DateTime.UtcNow - entity.CreatedAt).TotalSeconds < 5);
        Assert.True(entity.Active);
        Assert.Null(entity.UpdatedAt);
        Assert.Null(entity.UnActivateDate);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ShouldKeepActiveAndSetUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        Assert.True(entity.Active);

        // Act
        entity.Activate();

        // Assert
        Assert.True(entity.Active);
        Assert.Null(entity.UnActivateDate);
        Assert.NotNull(entity.UpdatedAt);
        Assert.True((DateTime.UtcNow - entity.UpdatedAt.Value).TotalSeconds < 5);
    }

    [Fact]
    public void Activate_WhenInactive_ShouldSetActiveTrueAndClearUnActivateDate()
    {
        // Arrange
        var entity = new TestEntity();
        entity.UnActivate();
        Assert.False(entity.Active);
        Assert.NotNull(entity.UnActivateDate);

        // Act
        entity.Activate();

        // Assert
        Assert.True(entity.Active);
        Assert.Null(entity.UnActivateDate);
        Assert.NotNull(entity.UpdatedAt);
        Assert.True((DateTime.UtcNow - entity.UpdatedAt.Value).TotalSeconds < 5);
    }

    [Fact]
    public void UnActivate_WhenActive_ShouldSetActiveFalseAndSetUnActivateDate()
    {
        // Arrange
        var entity = new TestEntity();
        Assert.True(entity.Active);

        // Act
        entity.UnActivate();

        // Assert
        Assert.False(entity.Active);
        Assert.NotNull(entity.UnActivateDate);
        Assert.True((DateTime.UtcNow - entity.UnActivateDate.Value).TotalSeconds < 5);
        Assert.NotNull(entity.UpdatedAt);
        Assert.True((DateTime.UtcNow - entity.UpdatedAt.Value).TotalSeconds < 5);
    }

    [Fact]
    public void UnActivate_WhenAlreadyInactive_ShouldKeepInactiveAndSetUnActivateDateAndUpdatedAt()
    {
        // Arrange
        var entity = new TestEntity();
        entity.UnActivate();
        var initialUnActivateDate = entity.UnActivateDate;
        var initialUpdatedAt = entity.UpdatedAt;

        // Act
        entity.UnActivate();

        // Assert
        Assert.False(entity.Active);
        Assert.NotNull(entity.UnActivateDate);
        Assert.True((DateTime.UtcNow - entity.UnActivateDate.Value).TotalSeconds < 5);
        Assert.NotNull(entity.UpdatedAt);
        Assert.True((DateTime.UtcNow - entity.UpdatedAt.Value).TotalSeconds < 5);
    }
}
