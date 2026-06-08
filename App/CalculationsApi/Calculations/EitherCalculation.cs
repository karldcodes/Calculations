using System.ComponentModel.DataAnnotations;

// These calculations assume that they are independent events as specified by the provided formulae.
public class EitherCalculation : Calculation<CalculationRequest, CalculationResponse>
{
    public override string Name => "Either";

    protected override Task<CalculationResponse> CalculateAsync(CalculationRequest request)
    {
        // P(A) + P(B) – P(A)P(B)
        var result = request.ProbabilityA + request.ProbabilityB - (request.ProbabilityA * request.ProbabilityB);
        return Task.FromResult(new CalculationResponse
        {
            Value = result
        });
    }

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
}
