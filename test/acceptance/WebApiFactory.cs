using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ChurchRota.Library.Data;

namespace church_rota.acceptance.tests;

// WebApplicationFactory for the API with an in-memory EF database for tests
public class WebApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ChurchRotaContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for tests. Use a unique name so parallel tests don't share state.
            var dbName = "TestDb_" + Guid.NewGuid().ToString("N");

            // Create an isolated EF service provider for the InMemory provider so it doesn't conflict
            // with the SQL Server provider services registered by the app.
            var inMemoryServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<ChurchRotaContext>(options =>
            {
                options.UseInMemoryDatabase(dbName)
                       .UseInternalServiceProvider(inMemoryServiceProvider);
            });

            // Build the service provider and ensure DB is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ChurchRotaContext>();
            db.Database.EnsureCreated();

            // Seed test data: a Person called "Mister Magoo" and a Role called "Reader"
            if (!db.People.Any(p => p.FirstName == "Mister" && p.LastName == "Magoo"))
            {
                db.People.Add(new Person
                {
                    FirstName = "Mister",
                    LastName = "Magoo",
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                });
            }

            if (!db.Roles.Any(r => r.RoleName == "Reader"))
            {
                db.Roles.Add(new Role
                {
                    RoleName = "Reader",
                    Description = "Test role",
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                });
            }

            db.SaveChanges();
        });

        // Use a non-development environment to avoid the Developer Exception Page during tests
        builder.UseEnvironment("Testing");
    }
}
