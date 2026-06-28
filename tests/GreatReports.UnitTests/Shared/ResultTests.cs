using GreatReports.Shared;
using System;
using Xunit;

namespace GreatReports.UnitTests.Shared;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult_ForNonGenericResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult_ForNonGenericResult()
    {
        // Arrange
        var error = new Error("Test.Error", "Ocorreu um erro de teste.");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Success_ShouldCreateSuccessfulResult_ForGenericResultUsingNonGenericMethod()
    {
        // Arrange
        var value = "Test Value";

        // Act
        var result = Result.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Success_ShouldCreateSuccessfulResult_ForGenericResult()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult_ForGenericResult()
    {
        // Arrange
        var error = new Error("Test.Error", "Erro genérico.");

        // Act
        var result = Result<string>.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailureResult_ForGenericResultUsingNonGenericMethod()
    {
        // Arrange
        var error = new Error("Test.Error", "Erro genérico.");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Value_ShouldThrowInvalidOperationException_WhenResultIsFailure()
    {
        // Arrange
        var error = new Error("Test.Error", "Erro genérico.");
        var result = Result<string>.Failure(error);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _ = result.Value);
        Assert.Equal("The value of a failure result cannot be accessed.", exception.Message);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertValueToSuccessResult_WhenValueIsNotNull()
    {
        // Arrange
        var value = "Implicit Value";

        // Act
        Result<string> result = value;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertNullToFailureResult_WhenValueIsNull()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("Value.Null", result.Error.Code);
        Assert.Equal("O valor fornecido é nulo.", result.Error.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowInvalidOperationException_WhenSuccessHasError()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            new TestResult(true, new Error("Invalid.Success", "Erro em sucesso.")));
        Assert.Equal("Success result cannot have an error.", exception.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowInvalidOperationException_WhenFailureHasNoError()
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            new TestResult(false, Error.None));
        Assert.Equal("Failure result must have an error.", exception.Message);
    }

    private class TestResult : Result
    {
        public TestResult(bool isSuccess, Error error) : base(isSuccess, error)
        {
        }
    }
}
