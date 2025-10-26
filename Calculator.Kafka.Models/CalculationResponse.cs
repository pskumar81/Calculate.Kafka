namespace Calculator.Kafka.Models;

public class CalculationResponse
{
    public Guid Id { get; set; }
    public double Result { get; set; }
    public string Operation { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public DateTime ProcessedTime { get; set; } = DateTime.UtcNow;
}