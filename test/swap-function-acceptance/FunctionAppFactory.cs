using Microsoft.Extensions.DependencyInjection;
using ChurchRota.SwapFunction.Repositories;

namespace ChurchRota.SwapFunction.Acceptance.Tests;

/// <summary>
/// Simple test setup helper that provides in-memory repositories for testing.
/// Azure Functions don't use WebApplicationFactory the same way as ASP.NET Core,
/// so we use this simpler approach to set up dependencies for testing.
/// </summary>
public static class TestSetup
{
    public static IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Register in-memory test repositories
        services.AddSingleton<IScheduleRepository, InMemoryScheduleRepository>();
        services.AddSingleton<IPeopleRepository, InMemoryPeopleRepository>();
        services.AddSingleton<IAvailabilityRepository, InMemoryAvailabilityRepository>();
        
        return services.BuildServiceProvider();
    }
}
