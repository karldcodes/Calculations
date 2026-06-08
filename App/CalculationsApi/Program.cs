using CalculationsApi.Logger;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);


// Add basic opentelemetry implementation with console logging for local development
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(Environment.GetEnvironmentVariable("serviceName") ?? "CalculationsApi"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            //.AddConsoleExporter() Dont show metrics in console as it masks logs
            .AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter();
    });

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.AddConsoleExporter();
    logging.AddOtlpExporter();
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ICalculationFactory,  CalculationFactory>();

// todo swap this for run time agreegation so we dont need to manually register these
builder.Services.AddSingleton<ICalculation, CombinedWithCalculation>();
builder.Services.AddSingleton<ICalculation, EitherCalculation>();

// Create meta data from all the calculations we have in the assembly. This is done once at start up and then calls from FE to
// get this list on rerenders should be fast. This assumes calcs are classes and not dynamically added!
builder.Services.AddSingleton<IReadOnlyList<CalculationMetadata>>(serviceProvider =>
{
    var calculations = serviceProvider.GetRequiredService<IEnumerable<ICalculation>>();

    return calculations
        .Select(CalculationMetadataFactory.Create)
        .ToList();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// return all the data needed for the FE project to display the correct infomation about the calculations
app.MapGet("/calculations", async (IReadOnlyList<CalculationMetadata> metadata) => Results.Ok(metadata));


// A cache could be added here if we know all calculations are deterministic and start becoming expensive to run
app.MapPost("/calculations/{name}", async (
    string name,
    JsonObject request,
    ICalculationFactory calculationFactory,
    ILogger<Program> logger
    ) =>
{
    CalculationsLog.RequestStarted(logger, name, request.ToString());

    // implement stratergy pattern combined with a factory for using calculations
    var calculation = calculationFactory.Get(name);

    if (calculation is null)
    {
        CalculationsLog.NotFound(logger, name);
        return Results.NotFound($"Unknown calculation: {name}");
    }

    // Generic cast from strongly typed abstract class
    object? typedRequest;
    try
    {
        typedRequest = request.Deserialize(calculation.RequestType, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow
        });
    }
    catch (JsonException)
    {
        CalculationsLog.InvalidRequest(logger, name);
        // fails if json body doesnt match what the requested calculation expected
        return Results.BadRequest("Invalid request JSON.");
    }

    try
    {
        // execute calculation
        var result = await calculation.ExecuteAsync(typedRequest!);
        CalculationsLog.RequestComplete(logger, name, JsonSerializer.Serialize(result));
        return Results.Ok(result);
    }
    catch (CalculationValidationException ex)
    {
        CalculationsLog.FailedValidation(logger, name, ex.Message, JsonSerializer.Serialize(ex.Errors));
        return Results.ValidationProblem(ex.Errors);
    }
});


app.Run();

// Allows us to use WebApplicationFactory in unit tests
public partial class Program { }