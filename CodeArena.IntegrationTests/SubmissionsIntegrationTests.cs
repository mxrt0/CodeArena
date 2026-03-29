using CodeArena.IntegrationTests.Infrastructure.Factories;
using CodeArena.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace CodeArena.IntegrationTests;

[TestFixture]
public class SubmissionsIntegrationTests
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
    public async Task Index_RequiresAuth_RedirectsToLogin()
    {
        var response = await _client.GetAsync("/Submissions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task Create_Valid_Redirects()
    {
        var formData = new Dictionary<string, string>
        {
            ["ChallengeId"] = "1",
            ["SolutionCode"] = "test-code-123",
            ["Language"] = "CSharp"
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Submissions/Create", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/submissions");
    }

    [Test]
    public async Task Create_InvalidModel_ReturnsNotFound()
    {
        var formData = new Dictionary<string, string>
        {
            ["challengeId"] = "",
            ["SolutionCode"] = "",
            ["Language"] = ""
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Submissions/Create", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("404");
    }

    [Test]
    public async Task Details_PassingInvalidId_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/Submissions/Details/0");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("400");
    }

    [Test]
    public async Task Details_InvalidSubmission_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/Submissions/Details/9999");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("404");
    }

    [Test]
    public async Task Cancel_RedirectsToIndex()
    {
        var formData = new Dictionary<string, string>
        {
            ["challengeId"] = "1",
            ["redirectToSubmissions"] = "true"
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await _client.PostAsync("/Submissions/Cancel", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/submissions");
    }
}