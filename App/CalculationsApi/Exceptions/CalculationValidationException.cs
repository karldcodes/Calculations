
public sealed class CalculationValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public CalculationValidationException(
        Dictionary<string, string[]> errors)
        : base("Calculation request validation failed.")
    {
        Errors = errors;
    }
}