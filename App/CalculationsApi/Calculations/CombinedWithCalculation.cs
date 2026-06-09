using CalculationsApi.Validation;
using System.ComponentModel.DataAnnotations;

public class CombinedWithCalculation : Calculation<CalculationRequest, CalculationResponse>
{
    public override string Name => "CombinedWith";

    protected override void Validate(CalculationRequest request)
    {
        var validator = new DataAttributeValidator();
        var result = validator.Validate(request);

        if(result.Any())
        {
            throw new CalculationValidationException(result);
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
