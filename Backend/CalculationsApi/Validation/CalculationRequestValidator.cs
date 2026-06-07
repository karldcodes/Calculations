using FluentValidation;
// Fluent validation to validate the request
public class CalculationRequestValidator : AbstractValidator<CalculationRequest>
{
    public CalculationRequestValidator()
    {
        RuleFor(m => m.ProbabilityA).InclusiveBetween(0, 1);
        RuleFor(m => m.ProbabilityB).InclusiveBetween(0, 1);
    }
}
