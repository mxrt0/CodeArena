using CodeArena.Common.Exceptions;
using CodeArena.Data;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Services.Core.Admin;
using CodeArena.Services.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeArena.Services.Tests;

[TestFixture]
public class AdminSubmissionServiceTests
{
    private Mock<IMemoryCache> _cacheMock;
    private ApplicationDbContext _context;
    private SubmissionRepository _repo;
    [SetUp]
    public async Task Setup()
    {
        _context = await DbContextFactory.CreateWithDataAsync(CreateSampleData());
        _repo = new SubmissionRepository(_context);

        var cacheEntry = new Mock<ICacheEntry>();
        cacheEntry.SetupGet(e => e.ExpirationTokens).Returns(new List<IChangeToken>());
        cacheEntry.SetupGet(e => e.PostEvictionCallbacks).Returns(new List<PostEvictionCallbackRegistration>());

        _cacheMock = new Mock<IMemoryCache>();
        _cacheMock
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);
        _cacheMock
            .Setup(c => c.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);
    }
    [Test]
    public async Task ApproveAsync_WhenPendingSubmission_ApprovesSuccessfully()
    {
        var service = new AdminSubmissionService(_repo, _cacheMock.Object);

        await service.ApproveAsync(1, "Good job");

        var updated = await _context.Submissions.FindAsync(1);
        Assert.That(updated?.Status, Is.EqualTo(SubmissionStatus.Approved));
        Assert.That(updated.Feedback, Is.EqualTo("Good job"));
    }

    [Test]
    public void ApproveAsync_WhenNotFound_ThrowsNotFound()
    {
        Assert.ThrowsAsync<SubmissionNotFoundException>(async () =>
        {
            var service = new AdminSubmissionService(_repo, _cacheMock.Object);

            await service.ApproveAsync(12345);
        });
    }

    [Test]
    public void ApproveAsync_WhenAlreadyApproved_ThrowsAlreadyApproved()
    {
        Assert.ThrowsAsync<SubmissionAlreadyApprovedException>(async () =>
        {
            var service = new AdminSubmissionService(_repo, _cacheMock.Object);

            await service.ApproveAsync(2);
        });
    }

    [Test]
    public async Task RejectAsync_WhenPendingSubmission_RejectsSuccessfully()
    {
        var service = new AdminSubmissionService(_repo, _cacheMock.Object);

        await service.RejectAsync(1, "Bad solution");

        var updated = await _context.Submissions.FindAsync(1);
        Assert.That(updated?.Status, Is.EqualTo(SubmissionStatus.Rejected));
        Assert.That(updated.Feedback, Is.EqualTo("Bad solution"));
    }

    [Test]
    public void RejectAsync_WhenAlreadyApproved_ThrowsAlreadyApproved()
    {
        Assert.ThrowsAsync<SubmissionAlreadyApprovedException>(async () =>
        {
            var service = new AdminSubmissionService(_repo, _cacheMock.Object);

            await service.RejectAsync(2);
        });
    }

    [Test]
    public async Task GetPendingSubmissionsAsync_WhenCalled_ReturnsOnlyPending()
    {
        var service = new AdminSubmissionService(_repo, _cacheMock.Object);

        var (result, count) = await service.GetPendingSubmissionsAsync();

        Assert.That(count, Is.EqualTo(1));
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().SubmissionId, Is.EqualTo(1));
    }

    [Test]
    public async Task GetPendingSubmissionsAsync_WithPagination_ReturnsCorrectPage()
    {
        var data = Enumerable.Range(1, 20)
            .Select(i => new Submission
            {
                Id = i,
                Status = SubmissionStatus.Pending,
                SolutionCode = "code",
                SubmittedAt = DateTime.UtcNow.AddMinutes(-i),
                UserId = $"user{i}",
                User = new ApplicationUser { Id = $"user{i}", DisplayName = $"User {i}" },
                Challenge = new Challenge
                {
                    Id = i,
                    Title = $"Challenge {i}",
                    Difficulty = Difficulty.Easy,
                    Description = $"Desc{i}",
                    Tags = $"tag{i}",
                    Slug = $"slug{i}"
                }
            }).ToList();

        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
 
        var service = new AdminSubmissionService(repo, _cacheMock.Object);

        var (result, count) = await service.GetPendingSubmissionsAsync(page: 2, pageSize: 5);

        Assert.That(count, Is.EqualTo(20));
        Assert.That(result.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetSubmissionForReviewAsync_WhenExists_ReturnsDto()
    {
        var service = new AdminSubmissionService(_repo, _cacheMock.Object);

        var dto = await service.GetSubmissionForReviewAsync(1);

        Assert.That(dto.SubmissionId, Is.EqualTo(1));
        Assert.That(dto.ChallengeTitle, Is.EqualTo("Challenge 1"));
        Assert.That(dto.UserDisplayName, Is.EqualTo("User One"));
    }

    [Test]
    public void GetSubmissionForReviewAsync_WhenNotFound_ThrowsNotFound()
    {
        Assert.ThrowsAsync<SubmissionNotFoundException>(async () =>
        {
            var service = new AdminSubmissionService(_repo, _cacheMock.Object);

            await service.GetSubmissionForReviewAsync(12345);
        });
    }

    private List<Submission> CreateSampleData()
    {
        return new List<Submission>
        {
            new Submission
            {
                Id = 1,
                Status = SubmissionStatus.Pending,
                SolutionCode = "code1",
                SubmittedAt = DateTime.UtcNow,
                UserId = "user1",
                User = new ApplicationUser { Id = "user1", DisplayName = "User One" },
                Challenge = new Challenge
                {
                    Id = 1,
                    Title = "Challenge 1",
                    Difficulty = Difficulty.Easy,
                    Description = "Desc1",
                    Tags = "tag1",
                    Slug = "slug1"
                }
            },
            new Submission
            {
                Id = 2,
                Status = SubmissionStatus.Approved,
                SolutionCode = "code2",
                SubmittedAt = DateTime.UtcNow.AddDays(-1),
                UserId = "user2",
                User = new ApplicationUser { Id = "user2", DisplayName = "User Two" },
                Challenge = new Challenge
                {
                    Id = 2,
                    Title = "Challenge 2",
                    Difficulty = Difficulty.Medium,
                    Description = "Desc2",
                    Tags = "tag2",
                    Slug = "slug2"
                }
            }
        };
    }
}