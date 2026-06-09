
using System.ComponentModel.DataAnnotations;

// This is being used by both combinedWith and either but this doesnt have to calculations can have their own classes
public class CalculationRequest {
    [Required]
    [Display(Name = "Probability A")]
    [Range(typeof(decimal), "0", "1", ErrorMessage = "Probability must be between 0 and 1.")]
    public decimal ProbabilityA { get; set; }
    [Required]
    [Display(Name = "Probability B")]
    [Range(typeof(decimal), "0", "1", ErrorMessage = "Probability must be between 0 and 1.")]
    public decimal ProbabilityB { get; set; }
}

