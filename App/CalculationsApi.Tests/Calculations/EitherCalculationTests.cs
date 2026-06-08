
namespace CalculationsApi.Tests.Calculations
{
    public class EitherCalculationTests
    {
        [Theory]
        [InlineData(0.5, 0.5, 0.75)]
        [InlineData(0, 0, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 0, 1)]
        public async Task Either_PrecisionEdgeCases_ReturnsCorrectValue(decimal A, decimal B, decimal expected)
        {
            var calc = new EitherCalculation();

            var reault = await calc.ExecuteAsync(new CalculationRequest
            {
                ProbabilityA = A,
                ProbabilityB = B
            });

            Assert.Equal(expected, ((CalculationResponse)reault!).Value);
        }
    }
}
