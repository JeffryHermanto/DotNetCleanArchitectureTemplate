using CleanArchitecture.Api;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    {
        Log.Information(
          "Starting application in {Environment} on {MachineName}",
          builder.Environment.EnvironmentName,
          Environment.MachineName
        );

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAppDI(builder.Configuration);
    }

    var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Title = "Clean Architecture API Docs";
                options.Theme = ScalarTheme.Default;
            });
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Lifetime.ApplicationStarted.Register(() =>
            Log.Information("Listening on {Urls}", string.Join(", ", app.Urls))
        );
        app.Run();
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}
