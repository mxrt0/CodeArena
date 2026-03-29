using CodeArena.Data;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.IntegrationTests.Infrastructure.Factories;
using CodeArena.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.IntegrationTests.Admin;

[TestFixture]
public class AdminChallengesIntegrationTests
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
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

        db.Challenges.Add(new Challenge
        {
            Id = 1,
            Title = "Testdfgd",
            Difficulty = Difficulty.Easy,
            Description = "Desc12324",
            Slug = "testdfgd",
            Tags = "tag123"
        });
        db.SaveChanges();
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

        var response = await unauthClient.GetAsync("/Admin/Challenges");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/Login");
    }

    [Test]
    public async Task Index_WithAdminAuth_ReturnsView()
    {
        var response = await _client.GetAsync("/Admin/Challenges");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Challenges");
    }

    [Test]
    public async Task CreateGet_ReturnsView()
    {
        var response = await _client.GetAsync("/Admin/Challenges/Create");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("<form");
    }

    [Test]
    public async Task CreatePost_ValidModel_Redirects()
    {
        var formData = new Dictionary<string, string>
        {
            ["Challenge.Title"] = "Test Challenge",
            ["Challenge.Description"] = "Description123",
            ["Challenge.Difficulty"] = "Easy",
            ["Challenge.Tags"] = "tag1,tag2"
        };
        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Admin/Challenges/Create", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/challenges");
    }

    [Test]
    public async Task EditGet_ExistingChallenge_ReturnsView()
    {
        var response = await _client.GetAsync("/Admin/Challenges/Edit/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Title");
    }

    [Test]
    public async Task EditGet_InvalidChallenge_RedirectsToIndex()
    {
        var response = await _client.GetAsync("/Admin/Challenges/Edit/9999");
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/challenges");
    }

    [Test]
    public async Task DeletePost_ExistingChallenge_Redirects()
    {
        var formData = new Dictionary<string, string>();
        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Admin/Challenges/Delete/1", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/challenges");
    }

    [Test]
    public async Task RestorePost_ExistingChallenge_Redirects()
    {
        var formData = new Dictionary<string, string>();
        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Admin/Challenges/Restore/1", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/challenges");
    }
}
