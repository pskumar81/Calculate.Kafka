namespace Calculator.Kafka.Models;

public class CalculationRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double FirstNumber { get; set; }
    public double SecondNumber { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
}