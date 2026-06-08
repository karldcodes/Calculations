using FluentValidation;

// These calculations assume that they are independent events as specified by the provided formulae.
public class EitherCalculation : Calculation<CalculationRequest, CalculationResponse>
{
    private readonly IValidator<CalculationRequest> validator;

    public EitherCalculation(IValidator<CalculationRequest> validator)
    {
        this.validator = validator;
    }
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
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var error in validationResult.Errors)
            {
                errors.Add(error.PropertyName, [error.ErrorMessage]);
            }

            throw new CalculationValidationException(errors);
        }
    }
}
