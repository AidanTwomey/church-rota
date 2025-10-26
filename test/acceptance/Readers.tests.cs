using System.Net;
using System.Text.Json;
using ChurchRota.Api.Model;
using Shouldly;

namespace church_rota.acceptance.tests;

public class ReadersTests : IClassFixture<WebApiFactory>
{
    private readonly WebApiFactory _factory;

    public ReadersTests(WebApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Readers_Returns201AndContainsReader()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/readers");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var readers = JsonSerializer.Deserialize<Reader[]>(json, options);
        
        readers.Single().Name.ShouldBe("Mister Magoo");
    }
}
