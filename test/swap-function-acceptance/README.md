# Testing Azure Functions - The Equivalent of WebApplicationFactory

For Azure Functions, there are **two main approaches** to acceptance testing:

## Option 1: Direct Unit Testing with In-Memory Repositories ✅ (Recommended)

This is the simplest and most reliable approach for Azure Functions using the isolated worker model.

**Key Differences from ASP.NET Core:**
- No equivalent to `WebApplicationFactory` - Azure Functions use a different hosting model
- Direct instantiation of function classes and services with test dependencies
- Mock or in-memory implementations of repositories/data access

**Benefits:**
- Simple and reliable
- Fast test execution
- Easy to debug
- Works well with isolated worker model

**Example structure:**
```csharp
public class SwapServiceTests
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IReaderMatchingService _matchingService;
    
    public SwapServiceTests()
    {
        // Use in-memory repositories (already implemented in this project)
        _scheduleRepository = new InMemoryScheduleRepository();
        _peopleRepository = new InMemoryPeopleRepository();
        _availabilityRepository = new In MemoryAvailabilityRepository();
        
        // Create real services with test dependencies
        _matchingService = new ReaderMatchingService(
            _peopleRepository,
            _availabilityRepository,
            _scheduleRepository);
    }
    
    [Fact]
    public async Task MatchingService_FindsAvailableReplacement()
    {
        // Arrange: Seed test data
        await SeedTestData();
        
        // Act: Call the service directly
        var replacement = await _matchingService.FindReplacementAsync(
            "schedule1", "reader-role", DateTime.Parse("2025-12-25"));
        
        // Assert: Verify results
        replacement.ShouldNotBeNull();
        replacement.Name.ShouldBe("Jane Reader");
    }
}
```

## Option 2: Integration Testing with Real Azure Storage Emulator

For true end-to-end tests, you can use Azurite (Azure Storage Emulator).

**Setup:**
```bash
# Install Azurite
npm install -g azurite

# Start Azurite
azurite --silent --location /tmp/azurite
```

**In tests:**
```csharp
// Point to Azurite
Environment.SetEnvironmentVariable(
    "TableStorageConnectionString", 
    "UseDevelopmentStorage=true");

// Use real repositories (they'll hit Azurite)
var scheduleRepo = new ScheduleRepository();
var peopleRepo = new PeopleRepository();
```

## Option 3: HTTP Testing with Custom Test Server (Complex, Not Recommended)

You can use `Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore` to host functions in a test server, but this requires:
- Additional packages
- Complex setup
- ASP.NET Core integration mode
- May not reflect actual Azure Functions behavior

**We don't recommend this approach** because:
- The isolated worker model doesn't map perfectly to ASP.NET Core
- Mocking `HttpRequestData` is complex and brittle
- Testing services/repositories directly is more reliable
- Option 1 (above) gives you 95% of the value with 10% of the complexity

## Summary

**For this project:**
- ✅ Use **Option 1** for business logic testing (matching service, repositories)
- ✅ Use **Option 2** for integration tests if needed (with Azurite)
- ❌ Avoid HTTP-level mocking - it's complex and provides little extra value

The `InMemoryRepositories.cs` file in this project provides everything you need for Option 1.
