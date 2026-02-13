using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ChurchRota.SwapFunction.Services;
using ChurchRota.SwapFunction.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register repositories
        services.AddSingleton<IScheduleRepository, ScheduleRepository>();
        services.AddSingleton<IPeopleRepository, PeopleRepository>();
        services.AddSingleton<IAvailabilityRepository, AvailabilityRepository>();

        // Register services
        services.AddSingleton<IReaderMatchingService, ReaderMatchingService>();
    })
    .Build();

host.Run();

// Make Program accessible to tests
public partial class Program { }
