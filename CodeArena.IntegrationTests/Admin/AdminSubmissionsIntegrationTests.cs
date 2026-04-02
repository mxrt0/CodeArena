using CodeArena.Data;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.IntegrationTests.Infrastructure.Factories;
using CodeArena.Web;
using CodeArena.Web.Areas.Admin.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.IntegrationTests.Admin;

[TestFixture]
public class AdminSubmissionsIntegrationTests
{
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private int _submissionId = default;

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

        db.Challenges.Add(new Challenge { Id = 1, Title = "Testdfgd",
            Difficulty = Difficulty.Easy,
            Description = "Desc12324", Slug = "testdfgd",
            Tags = "tag123"});

        db.Users.Add(new ApplicationUser
        {
            Id = "test-user-id",
            UserName = "test-user",
            Email = "test@test.com",
            DisplayName = "ImTestUser",
            NormalizedDisplayName = "IMTESTUSER"
        });
        db.SaveChanges();

        if (!db.Submissions.Any())
        {
            var submission = new Submission
            {
                ChallengeId = 1,
                UserId = "test-user-id",
                SolutionCode = "TestCode",
                Language = SubmissionLanguage.CSharp,
                Status = SubmissionStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };
            db.Submissions.Add(submission);
            db.SaveChanges();
            _submissionId = submission.Id;
        }
        else
        {
            _submissionId = db.Submissions.First().Id;
        }
    }

    [TearDown]
    public void Cleanup()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task Index_ReturnsOkAndContainsSubmissionsPage()
    {
        var response = await _client.GetAsync("/Admin/Submissions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Submissions");
    }

    [Test]
    public async Task Review_ValidId_ReturnsView()
    {
 
        var response = await _client.GetAsync($"/Admin/Submissions/Review/{_submissionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Submission");
    }

    [Test]
    public async Task Review_InvalidId_RedirectsToIndex()
    {
        var response = await _client.GetAsync("/Admin/Submissions/Review/9999");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/submissions");
    }

    [Test]
    public async Task Approve_ValidSubmission_RedirectsToIndex()
    {
        var formData = new Dictionary<string, string>
        {
            ["Submission.SubmissionId"] = _submissionId.ToString(),
            ["SubmissionFeedback"] = "Good job"
        };

        var content = new FormUrlEncodedContent(formData);
        var response = await _client.PostAsync("/Admin/Submissions/Approve", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/submissions");
    }

    [Test]
    public async Task Reject_ValidSubmission_RedirectsToIndex()
    {
        var formData = new Dictionary<string, string>
        {
            ["Submission.SubmissionId"] = _submissionId.ToString(),
            ["SubmissionFeedback"] = "Not good at all"
        };

        var content = new FormUrlEncodedContent(formData);
        var response = await _client.PostAsync("/Admin/Submissions/Reject", content);

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.ToString().Should().Contain("/admin/submissions");
    }
}
