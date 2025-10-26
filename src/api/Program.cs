using Microsoft.EntityFrameworkCore;
using ChurchRota.Library.Data;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Register DbContext. Connection string is read from Configuration: ConnectionStrings:DefaultConnection
// You can override it with the environment variable CONNECTION_STRING if needed.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? "Server=host.docker.internal;Database=ChurchRota;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True";

builder.Services.AddDbContext<ChurchRotaContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// On startup, create the database and apply any pending migrations (or create schema when no migrations exist)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<ChurchRotaContext>();
        // Try to apply migrations; if there are none or migrations infrastructure isn't used in this repo,
        // fall back to EnsureCreated which creates the schema from the model.
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception migrateEx)
        {
            logger.LogWarning(migrateEx, "Applying migrations failed, falling back to EnsureCreated().");
            context.Database.EnsureCreated();
            logger.LogInformation("Database created (EnsureCreated).");
        }
    }
    catch (Exception ex)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var log = loggerFactory.CreateLogger("Program");
        log.LogError(ex, "An error occurred while migrating or initializing the database.");
        // Rethrow or swallow depending on whether you want the app to start when DB init fails.
        throw;
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
