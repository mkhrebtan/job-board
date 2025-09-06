using API.Extensions;
using Api.Middlewares.Exceptions;
using Application;
using Application.Abstractions.Messaging;
using Application.TestData.Commands.TestCommand;
using Application.TestData.Queries.TestQuery;
using Persistence;
using Serilog;
using Serilog.Events;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .Enrich.FromLogContext()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting up");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

            builder.Services.AddOpenApi();

            builder.Services.AddPersistence(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                    setup.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.MapSwagger().RequireAuthorization();

            app.MapEndpoints();

            app.UseSerilogRequestLogging();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}