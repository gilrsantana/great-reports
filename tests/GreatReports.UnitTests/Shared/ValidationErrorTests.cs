using GreatReports.Shared;

namespace GreatReports.UnitTests.Shared;

public class ValidationErrorTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly_WhenProvidedWithErrors()
    {
        // Arrange
        var errors = new[]
        {
            new Error("Field.Required", "O campo é obrigatório."),
            new Error("Field.Invalid", "O valor fornecido é inválido.")
        };

        // Act
        var validationError = new ValidationError(errors);

        // Assert
        Assert.NotNull(validationError);
        Assert.Equal("Validation.Error", validationError.Code);
        Assert.Equal("Ocorreram um ou mais erros de validação.", validationError.Description);
        Assert.Equal(errors, validationError.Errors);
        Assert.Equal(2, validationError.Errors.Length);
    }

    [Fact]
    public void FromErrors_ShouldCreateValidationErrorCorrectly()
    {
        // Arrange
        var errors = new[]
        {
            new Error("Email.Required", "O e-mail é obrigatório.")
        };

        // Act
        var validationError = ValidationError.FromErrors(errors);

        // Assert
        Assert.NotNull(validationError);
        Assert.Equal("Validation.Error", validationError.Code);
        Assert.Equal("Ocorreram um ou mais erros de validação.", validationError.Description);
        Assert.Single(validationError.Errors);
        Assert.Equal(errors[0], validationError.Errors[0]);
    }

    [Fact]
    public void Constructor_ShouldHandleEmptyErrorsArray()
    {
        // Arrange
        var errors = Array.Empty<Error>();

        // Act
        var validationError = new ValidationError(errors);

        // Assert
        Assert.NotNull(validationError);
        Assert.Equal("Validation.Error", validationError.Code);
        Assert.Equal("Ocorreram um ou mais erros de validação.", validationError.Description);
        Assert.Empty(validationError.Errors);
    }
}
