public sealed class CalculationValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public CalculationValidationException(
        IReadOnlyDictionary<string, string[]> errors)
        : base("Calculation request validation failed.")
    {
        Errors = errors;
    }
}