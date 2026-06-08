
using System.ComponentModel.DataAnnotations;

// This is being used by both combinedWith and either but this doesnt have to
public class CalculationRequest {
    [Required]
    [Range(0, 1)]
    public decimal ProbabilityA { get; set; }
    [Required]
    [Range(0, 1)]
    public decimal ProbabilityB { get; set; }
}

