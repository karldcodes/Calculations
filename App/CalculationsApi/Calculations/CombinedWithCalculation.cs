using System.ComponentModel.DataAnnotations;

public class CombinedWithCalculation : Calculation<CalculationRequest, CalculationResponse>
{
    public override string Name => "CombinedWith";

    protected override void Validate(CalculationRequest request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            context,
            results,
            validateAllProperties: true);

        if (results.Any())
        {
            throw new CalculationValidationException(new Dictionary<string, string[]>());
        }
    }

    protected override Task<CalculationResponse> CalculateAsync(CalculationRequest request)
    {
        // P(A)P(B)
        var result = request.ProbabilityA * request.ProbabilityB;
        return Task.FromResult(new CalculationResponse
        {
            Value = result
        });
    }
}
