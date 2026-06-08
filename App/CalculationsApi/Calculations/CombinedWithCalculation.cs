using FluentValidation;

public class CombinedWithCalculation : Calculation<CalculationRequest, CalculationResponse>
{
    private readonly IValidator<CalculationRequest> validator;

    public CombinedWithCalculation(IValidator<CalculationRequest> validator)
    {
        this.validator = validator;
    }
    public override string Name => "CombinedWith";

    // validation logic isolated to the calculator class so domain rules stay within the same domain as the calculation
    // Here we are only using fluentvalidation but it could also be custom validation or 3rd party
    // the validation logic is duplicated in the other class which could be seen as breaking DRY but I didnt want to implement this in 
    // the abstract class as we would then always have to create a validator for IValidator<TResponse> which might not always be needed
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
