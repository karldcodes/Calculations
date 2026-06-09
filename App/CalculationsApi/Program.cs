using CalculationsApi.Logger;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add basic opentelemetry implementation with console logging for local development
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(Environment.GetEnvironmentVariable("serviceName") ?? "CalculationsApi"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
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


// This adds a cors policy so that only the FE can access the api but in a real app this would be handled by Azure API Management
const string FrontendPolicy = "FrontendPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:53434") // React FE in the real app this would come from appsettings etc
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// added as singleton so each request doesnt keep repeatable getting all calculations from DI
builder.Services.AddSingleton<ICalculationFactory,  CalculationFactory>(); 

// For this tech demo the calcs are manually registered but in a real app they could be dynamically added on startup
builder.Services.AddSingleton<ICalculation, CombinedWithCalculation>();
builder.Services.AddSingleton<ICalculation, EitherCalculation>();

// Create meta data from all the calculations we have in the assembly. This is done once at start up so calls from FE to
// get this list on any rerenders should be fast. This assumes calcs are classes and not dynamically added!
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

app.UseCors(FrontendPolicy);

// return all the meta data needed for the FE project to dynamically render the calculation form
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
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        });
    }
    catch (JsonException ex)
    {
        CalculationsLog.InvalidRequest(logger, ex, name);
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
        CalculationsLog.RequestComplete(logger, name, JsonSerializer.Serialize(result));
        return Results.Ok(result);
    }
    catch (CalculationValidationException ex)
    {
        CalculationsLog.FailedValidation(logger, ex, name, ex.Message, JsonSerializer.Serialize(ex.Errors));
        return Results.ValidationProblem(ex.Errors);
    }
});


app.Run();

// Allows us to use WebApplicationFactory in unit tests
public partial class Program { }