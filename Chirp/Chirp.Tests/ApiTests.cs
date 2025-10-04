namespace Chirp.Tests;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task PublicTimeline_Contains_HelgeCheep()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Helge");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Helge", content);
        Assert.Contains("Hello, BDSA students!", content);
    }
}