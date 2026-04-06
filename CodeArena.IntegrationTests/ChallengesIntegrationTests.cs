using CodeArena.Data;
using CodeArena.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CodeArena.IntegrationTests.Infrastructure.Factories;
namespace CodeArena.IntegrationTests;

[TestFixture]
public class ChallengesIntegrationTests
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
    public async Task Index_ReturnsSuccessAndContainsChallengeTitle()
    {
        var response = await _client.GetAsync("/challenges");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Balanced Brackets");
    }

    [Test]
    public async Task Index_WithPageQuery_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/challenges?page=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Details_ValidChallengeSlug_ReturnsSuccessAndContent()
    {
        string slug = "sum-two-numbers";

        var response = await _client.GetAsync($"/challenges/{slug}");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Sum Two Numbers");
    }

    [Test]
    public async Task Details_InvalidChallengeSlug_ReturnsNotFound()
    {
        string slug = "invalid-slug";

        var response = await _client.GetAsync($"/challenges/{slug}");
        var content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().Contain("Page Not Found");
    }
}
