using System.Text.Json.Serialization;

public class CalculationRequest {
    [JsonPropertyName("probabilityA")]
    public decimal ProbabilityA { get; set; }
    [JsonPropertyName("probabilityB")]
    public decimal ProbabilityB { get; set; }
}

