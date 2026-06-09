
namespace CalculationsApi.Tests.Calculations
{
    public class EitherCalculationTests
    {
        [Theory]
        [InlineData(0.5, 0.5, 0.75)]
        [InlineData(0, 0.5, 0.5)]
        [InlineData(1, 0.5, 1)]
        public async Task ExecuteAsync_ReturnsExpectedResult(
        decimal probabilityA,
        decimal probabilityB,
        decimal expected)
        {
            var calc = new EitherCalculation();

            var result = await calc.ExecuteAsync(new CalculationRequest
            {
                ProbabilityA = probabilityA,
                ProbabilityB = probabilityB
            });

            Assert.Equal(expected, ((CalculationResponse)result!).Value);
        }
    }
}
