using System.Text.Json.Nodes;

namespace CalculationsApi.RequestHandlers
{
    public interface ICalculationRequestHandler
    {
        Task<IResult> HandleAsync(string name, JsonObject request);
    }
}