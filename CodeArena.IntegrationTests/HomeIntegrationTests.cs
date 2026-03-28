using CodeArena.IntegrationTests.Infrastructure.Factories;
using CodeArena.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace CodeArena.IntegrationTests;

[TestFixture]
public class HomeIntegrationTests
{
    private HttpClient _client = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    [TearDown]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }
    [Test]
    public async Task Get_HomeIndex_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [TestCase(400, "Bad Request")]
    [TestCase(404, "Page Not Found")]
    [TestCase(403, "Forbidden")]
    [TestCase(500, "Server Error")]
    public async Task ErrorPages_ReturnCorrectView(int statusCode, string expectedHeading)
    {
        var response = await _client.GetAsync($"/Home/Error/{statusCode}");

        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain(expectedHeading);
    }
}
