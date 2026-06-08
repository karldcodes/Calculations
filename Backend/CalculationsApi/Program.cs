using FluentValidation;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

const string frontentOrigin = "frontendOrigin";

// setup cors policy so that only the frontend domain can access the backend services
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontentOrigin,
    policy =>
    {
        policy.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
    });
});


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IValidator<CalculationRequest>, CalculationRequestValidator>();
builder.Services.AddScoped<ICalculationFactory,  CalculationFactory>();

// todo swap this for run time agreegation so we dont need to manually register these
builder.Services.AddScoped<ICalculation, CombinedWithCalculation>();
builder.Services.AddScoped<ICalculation, EitherCalculation>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// todo metric around request duration and status codes etc

// A cache could be added here if we know all calculations are deterministic and start becoming expensive to run
app.MapPost("/calculation/{name}", async (
    string name,
    JsonElement request,
    ICalculationFactory calculationFactory
    ) =>
{
    // implement stratergy pattern combined with a factory for using calculations
    var calculation = calculationFactory.Get(name);

    if (calculation is null)
        return Results.NotFound($"Unknown calculation: {name}");

    // Generic cast from strongly typed abstract class
    object? typedRequest;
    try
    {
        typedRequest = request.Deserialize(calculation.RequestType);
    }
    catch (JsonException)
    {
        // fails if json body doesnt match what the requested calculation expected
        return Results.BadRequest("Invalid request JSON.");
    }

    try
    {
        // execute calculation
        var result = await calculation.ExecuteAsync(typedRequest!);
        return Results.Ok(result);
    }
    catch (CalculationValidationException ex)
    {
        return Results.ValidationProblem(ex.Errors);
    }
});

app.UseCors(frontentOrigin);

app.Run();

// Allows us to use WebApplicationFactory in unit tests
public partial class Program { }