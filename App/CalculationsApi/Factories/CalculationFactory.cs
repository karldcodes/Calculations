
/**
 * Calculation factory to find the correct calculation when requested. This could also be used later for other logic specific to
 * calculations such as listing total amount etc and other metadata. Calculators stored as list here but a better data structure 
 * could be used if calculation list grows e.g dictionary benchmarking would be recommended to find the best type
 */
public class CalculationFactory : ICalculationFactory
{
    private readonly IEnumerable<ICalculation> calculations;

    public CalculationFactory(IEnumerable<ICalculation> calculations)
    {
        this.calculations = calculations;
    }
    public ICalculation? Get(string name)
    {
        return calculations
            .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
