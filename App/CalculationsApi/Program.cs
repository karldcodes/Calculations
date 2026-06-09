using CalculationsApi.RequestHandlers;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json.Nodes;

public partial class Program
{
    private static void Main(string[] args)
    {
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
        builder.Services.AddSingleton<ICalculationFactory, CalculationFactory>();

        // For this tech demo the calcs are manually registered but in a real app they could be dynamically added on startup
        builder.Services.AddSingleton<ICalculation, CombinedWithCalculation>();
        builder.Services.AddSingleton<ICalculation, EitherCalculation>();
        builder.Services.AddScoped<ICalculationRequestHandler, CalculationRequestHandler>();

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


        /* 
         * in a simple project with only a few apis id keep it as a minimal api rather then creating controllers to save extra boilerplate
         */

        // return all the meta data needed for the FE project to dynamically render the calculation form
        app.MapGet("/calculations", async (IReadOnlyList<CalculationMetadata> metadata) => Results.Ok(metadata));

        // A cache could be added here if we know all calculations are deterministic and start becoming expensive to run
        app.MapPost("/calculations/{name}", async (
                    string name,
                    JsonObject request,
                    ICalculationRequestHandler handler) => await handler.HandleAsync(name, request));

        app.Run();
    }
}