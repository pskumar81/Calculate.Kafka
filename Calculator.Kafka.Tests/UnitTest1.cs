using Calculator.Kafka.Models;

namespace Calculator.Kafka.Tests;

public class CalculatorModelsTests
{
    [Fact]
    public void CalculationRequest_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var request = new CalculationRequest();

        // Assert
        Assert.NotEqual(Guid.Empty, request.Id);
        Assert.Equal(0, request.FirstNumber);
        Assert.Equal(0, request.SecondNumber);
        Assert.Equal(string.Empty, request.Operation);
        Assert.True(request.RequestTime <= DateTime.UtcNow);
    }

    [Fact]
    public void CalculationResponse_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var response = new CalculationResponse();

        // Assert
        Assert.Equal(Guid.Empty, response.Id);
        Assert.Equal(0, response.Result);
        Assert.Equal(string.Empty, response.Operation);
        Assert.False(response.IsSuccess);
        Assert.Equal(string.Empty, response.ErrorMessage);
        Assert.True(response.ProcessedTime <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(10, 5, "add", 15)]
    [InlineData(10, 5, "subtract", 5)]
    [InlineData(10, 5, "multiply", 50)]
    [InlineData(10, 5, "divide", 2)]
    public void CalculationRequest_ShouldAllowBasicOperations(double first, double second, string operation, double expected)
    {
        // Arrange
        var request = new CalculationRequest
        {
            FirstNumber = first,
            SecondNumber = second,
            Operation = operation
        };

        // Act
        var result = operation.ToLower() switch
        {
            "add" => request.FirstNumber + request.SecondNumber,
            "subtract" => request.FirstNumber - request.SecondNumber,
            "multiply" => request.FirstNumber * request.SecondNumber,
            "divide" => request.FirstNumber / request.SecondNumber,
            _ => throw new InvalidOperationException($"Unsupported operation: {operation}")
        };

        // Assert
        Assert.Equal(expected, result);
    }
}
