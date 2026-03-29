using CodeArena.IntegrationTests.Infrastructure.Factories;
using CodeArena.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.IntegrationTests.Admin;

[TestFixture]
public class AdminDashboardIntegrationTests
{
    private HttpClient _client = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [TearDown]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Index_WithoutAuth_RedirectsToLogin()
    {
        var unauthFactory = new WebApplicationFactory<Program>();
        var unauthClient = unauthFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await unauthClient.GetAsync("/Admin/Dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Login");
    }

    [Test]
    public async Task Index_WithAdminAuth_ReturnsView()
    {
        var response = await _client.GetAsync("/Admin/Dashboard");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Admin Dashboard");
    }

    [Test]
    public async Task Index_ViewContainsDashboardData()
    {
        var response = await _client.GetAsync("/Admin/Dashboard");
        var content = await response.Content.ReadAsStringAsync();

        content.Should().Contain("Total Users");
    }
}
