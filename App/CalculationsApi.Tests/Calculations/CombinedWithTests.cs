using System;
using System.Collections.Generic;
using System.Text;

namespace CalculationsApi.Tests.Calculations
{
    public class CombinedWithTests
    {
        [Theory]
        [InlineData(0.5, 0.5, 0.25)]
        [InlineData(0, 0.5, 0)]
        [InlineData(1, 0.5, 0.5)]
        public async Task ExecuteAsync_ReturnsExpectedResult(
        decimal probabilityA,
        decimal probabilityB,
        decimal expected)
        {
            var calculation = new CombinedWithCalculation();

            var result = await calculation.ExecuteAsync(
                new CalculationRequest
                {
                    ProbabilityA = probabilityA,
                    ProbabilityB = probabilityB
                });

            Assert.Equal(expected, ((CalculationResponse)result!).Value);
        }
    }
}
