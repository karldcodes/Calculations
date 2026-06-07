using System.Text.Json.Serialization;

public class CalculationRequest {
    [JsonPropertyName("probabilityA")]
    public float ProbabilityA { get; set; }
    [JsonPropertyName("probabilityB")]
    public float ProbabilityB { get; set; }
}

