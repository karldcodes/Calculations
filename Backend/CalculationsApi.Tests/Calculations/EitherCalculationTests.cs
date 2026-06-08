using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

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
            var validatorMock = new Mock<IValidator<CalculationRequest>>();
            validatorMock.Setup(x => x.Validate(It.IsAny<CalculationRequest>())).Returns(new FluentValidation.Results.ValidationResult()
            {
                Errors = new List<FluentValidation.Results.ValidationFailure>()
            });

            var calc = new EitherCalculation(validatorMock.Object);

            var reault = await calc.ExecuteAsync(new CalculationRequest
            {
                ProbabilityA = A,
                ProbabilityB = B
            });

            Assert.Equal(expected, ((CalculationResponse)reault!).Value);
        }
    }
}
