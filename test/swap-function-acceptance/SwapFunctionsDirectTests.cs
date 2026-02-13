using Shouldly;
using Xunit;
using ChurchRota.SwapFunction.Models;
using ChurchRota.SwapFunction.Repositories;
using ChurchRota.SwapFunction.Services;

namespace ChurchRota.SwapFunction.Acceptance.Tests;

/// <summary>
/// Service-level tests for the swap functionality.
/// This approach tests the business logic directly without HTTP mocking complexity.
/// This is the recommended approach for Azure Functions testing.
/// </summary>
public class SwapServiceTests
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IPeopleRepository _peopleRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IReaderMatchingService _matchingService;

    public SwapServiceTests()
    {
        // Create in-memory repositories with test data
        _scheduleRepository = new InMemoryScheduleRepository();
        _peopleRepository = new InMemoryPeopleRepository();
        _availabilityRepository = new InMemoryAvailabilityRepository();
        
        // Seed test data
        SeedTestData().Wait();

        _matchingService = new ReaderMatchingService(
            _peopleRepository,
            _availabilityRepository,
            _scheduleRepository);
    }

    private async Task SeedTestData()
    {
        // Add some test people with Reader role
        await _peopleRepository.CreateAsync(new PersonTableEntity
        {
            PersonId = "person1",
            Name = "John Reader",
            Phone = "07700900001",
            Email = "john@example.com",
            Roles = "reader-role"
        });

        await _peopleRepository.CreateAsync(new PersonTableEntity
        {
            PersonId = "person2",
            Name = "Jane Reader",
            Phone = "07700900002",
            Email = "jane@example.com",
            Roles = "reader-role"
        });

        await _peopleRepository.CreateAsync(new PersonTableEntity
        {
            PersonId = "person3",
            Name = "Bob Reader",
            Phone = "07700900003",
            Email = "bob@example.com",
            Roles = "reader-role"
        });

        // Add a schedule entry for person1
        await _scheduleRepository.CreateAsync(new ScheduleTableEntity
        {
            ScheduleId = "schedule1",
            Date = DateTime.Parse("2025-12-25"),
            PersonId = "person1",
            PersonName = "John Reader",
            RoleId = "reader-role",
            RoleName = "Reader",
            Status = "Confirmed",
            Solemnity = "Christmas"
        });

        // Mark person2 as available, person3 as unavailable
        await _availabilityRepository.SetAsync(new AvailabilityTableEntity
        {
            PersonId = "person2",
            Date = DateTime.Parse("2025-12-25"),
            IsAvailable = true
        });

        await _availabilityRepository.SetAsync(new AvailabilityTableEntity
        {
            PersonId = "person3",
            Date = DateTime.Parse("2025-12-25"),
            IsAvailable = false
        });
    }

    [Fact]
    public async Task FindReplacement_WithAvailableReader_ReturnsReplacement()
    {
        // Act
        var replacement = await _matchingService.FindReplacementAsync(
            "schedule1", 
            "reader-role", 
            DateTime.Parse("2025-12-25"));

        // Assert
        replacement.ShouldNotBeNull();
        replacement.PersonId.ShouldBe("person2"); // Jane is available
        replacement.Name.ShouldBe("Jane Reader");
        replacement.Phone.ShouldBe("07700900002");
        replacement.Email.ShouldBe("jane@example.com");
    }

    [Fact]
    public async Task FindReplacement_WhenNoAvailableReaders_ReturnsNull()
    {
        // Arrange: Make all readers unavailable
        await _availabilityRepository.SetAsync(new AvailabilityTableEntity
        {
            PersonId = "person2",
            Date = DateTime.Parse("2025-12-25"),
            IsAvailable = false
        });

        // Act
        var replacement = await _matchingService.FindReplacementAsync(
            "schedule1",
            "reader-role",
            DateTime.Parse("2025-12-25"));

        // Assert
        replacement.ShouldBeNull();
    }

    [Fact]
    public async Task FindReplacement_SelectsReaderWithFewestAssignments()
    {
        // Arrange: Give person2 many past assignments
        var threeMonthsAgo = DateTime.Parse("2025-09-25");
        for (int i = 0; i < 5; i++)
        {
            await _scheduleRepository.CreateAsync(new ScheduleTableEntity
            {
                ScheduleId = $"past-schedule-{i}",
                Date = threeMonthsAgo.AddDays(i * 7),
                PersonId = "person2",
                PersonName = "Jane Reader",
                RoleId = "reader-role",
                RoleName = "Reader",
                Status = "Confirmed"
            });
        }

        // Make person3 available (they have 0 assignments)
        await _availabilityRepository.SetAsync(new AvailabilityTableEntity
        {
            PersonId = "person3",
            Date = DateTime.Parse("2025-12-25"),
            IsAvailable = true
        });

        // Act
        var replacement = await _matchingService.FindReplacementAsync(
            "schedule1",
            "reader-role",
            DateTime.Parse("2025-12-25"));

        // Assert - should pick person3 because they have fewer assignments
        replacement.ShouldNotBeNull();
        replacement.PersonId.ShouldBe("person3"); // Bob has fewer assignments
        replacement.Name.ShouldBe("Bob Reader");
    }

    [Fact]
    public async Task FindAlternatives_ReturnsUpcomingAvailability()
    {
        // Arrange: Make someone available next week
        await _availabilityRepository.SetAsync(new AvailabilityTableEntity
        {
            PersonId = "person3",
            Date = DateTime.Parse("2026-01-01"),
            IsAvailable = true
        });

        // Act
        var alternatives = await _matchingService.FindAlternativesAsync(
            "reader-role",
            DateTime.Parse("2025-12-25"));

        // Assert
        alternatives.ShouldNotBeEmpty();
        var suggestion = alternatives.FirstOrDefault(a => a.PersonId == "person3");
        suggestion.ShouldNotBeNull();
        suggestion.AvailableFrom.ShouldBe(DateTime.Parse("2026-01-01"));
    }

    [Fact]
    public async Task ScheduleRepository_TracksAssignmentCount()
    {
        // Act
        var count = await _scheduleRepository.GetPersonAssignmentCountAsync(
            "person1",
            DateTime.Parse("2025-09-01"),
            DateTime.Parse("2025-12-31"));

        // Assert
        count.ShouldBe(1); // Only schedule1
    }
}
