using CalculationsApi.Logger;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CalculationsApi.RequestHandlers
{
    public class CalculationRequestHandler : ICalculationRequestHandler
    {
        private readonly ICalculationFactory _calculationFactory;
        private readonly ILogger<CalculationRequestHandler> _logger;

        public CalculationRequestHandler(
            ICalculationFactory calculationFactory,
            ILogger<CalculationRequestHandler> logger)
        {
            _calculationFactory = calculationFactory;
            _logger = logger;
        }

        public async Task<IResult> HandleAsync(string name, JsonObject request)
        {
            CalculationsLog.RequestStarted(_logger, name, request.ToString());

            // implement stratergy pattern combined with a factory for using calculations
            var calculation = _calculationFactory.Get(name);

            if (calculation is null)
            {
                CalculationsLog.NotFound(_logger, name);
                return Results.NotFound($"Unknown calculation: {name}");
            }

            // Generic cast from strongly typed abstract class
            object? typedRequest;
            try
            {
                typedRequest = request.Deserialize(calculation.RequestType, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });
            }
            catch (JsonException ex)
            {
                CalculationsLog.InvalidRequest(_logger, ex, name);
                // fails if json body doesnt match what the requested calculation expected.
                // send alert to SD team to take a look at the models coming from the client
                return Results.ValidationProblem(
                errors: new Dictionary<string, string[]>
                {
                    { "_generic", new[] { "Invalid request JSON." } }
                });
            }

            try
            {
                // execute calculation
                var result = await calculation.ExecuteAsync(typedRequest!);
                CalculationsLog.RequestComplete(_logger, name, JsonSerializer.Serialize(result));
                return Results.Ok(result);
            }
            catch (CalculationValidationException ex)
            {
                CalculationsLog.FailedValidation(_logger, ex, name, ex.Message, JsonSerializer.Serialize(ex.Errors));
                return Results.ValidationProblem(ex.Errors);
            }
        }
    }
}
