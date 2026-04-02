using CodeArena.Common;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Services.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Tests;

[TestFixture]
public class SubmissionServiceTests
{
    private string _sampleUserId = "user-123";
    private Mock<IMemoryCache> _cacheMock;

    [SetUp]
    public void Setup()
    {
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
    public async Task GetSubmissionDetailsAsync_WhenNoUser_ReturnsFailResult()
    {
        var repoMock = new Mock<ISubmissionRepository>();

        var service = new SubmissionService(repoMock.Object,
            _cacheMock.Object);

        var result = await service.GetSubmissionDetailsAsync(123, _sampleUserId);

        Assert.That(result.Success, Is.False);
    }
    [Test]
    public async Task GetSubmissionDetailsAsync_WhenNonExistentId_ReturnsFailResult()
    {
        var repoMock = new Mock<ISubmissionRepository>();
 
        repoMock
            .Setup(r => r.AnyAsync(s => s.Id == 1))
            .ReturnsAsync(false);

        var service = new SubmissionService(repoMock.Object,
            _cacheMock.Object);

        var result = await service.GetSubmissionDetailsAsync(1, _sampleUserId);

        Assert.That(result.Success, Is.False);
    }
    [Test]
    public async Task GetSubmissionDetailsAsync_WhenSubmissionDoesNotBelongToUser_ReturnsFailResult()
    {
        var repoMock = new Mock<ISubmissionRepository>();

        repoMock
            .Setup(r => r.AnyAsync(s => s.Id == 1))
            .ReturnsAsync(true);

        repoMock
            .Setup(r => r.AnyAsync(s => s.Id == 1 && s.UserId == _sampleUserId))
            .ReturnsAsync(false);

        var service = new SubmissionService(repoMock.Object,
            _cacheMock.Object);

        var result = await service.GetSubmissionDetailsAsync(1, _sampleUserId);

        Assert.That(result.Success, Is.False);
    }
    [Test]
    public async Task GetSubmissionDetailsAsync_WhenValidInput_CorrectlyReturnsSuccessResult()
    {
        var data = CreateSampleData(_sampleUserId);

        var context = await DbContextFactory.CreateWithDataAsync(data);

        var repo = new SubmissionRepository(context);

        var service = new SubmissionService(repo, _cacheMock.Object);

        var result = await service.GetSubmissionDetailsAsync(1, _sampleUserId);

        Assert.That(result.Success, Is.True);
    }
    [Test]
    public async Task GetUserSubmissionsAsync_ReturnsOnlyUserSubmissions_WithCorrectPaging()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);

        var service = new SubmissionService(repo, _cacheMock.Object);

        var result = await service.GetUserSubmissionsAsync(new(), _sampleUserId);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Items.Count(), Is.EqualTo(2));
        Assert.That(result.Items.All(d => d.ChallengeId is not 3));
    }
    [Test]
    public async Task HasApprovedSubmissionAsync_ReturnsCorrectly()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
  
        var service = new SubmissionService(repo, _cacheMock.Object);

        var result1 = await service.HasApprovedSubmissionAsync(1, _sampleUserId);
        var result2 = await service.HasApprovedSubmissionAsync(2, _sampleUserId);
        var result3 = await service.HasApprovedSubmissionAsync(3, _sampleUserId);

        Assert.That(result1, Is.True);
        Assert.That(result2, Is.False);
        Assert.That(result3, Is.False);
    }
    [Test]
    public async Task HasPendingSubmissionAsync_ReturnsCorrectly()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
 
        var service = new SubmissionService(repo, _cacheMock.Object);

        var result1 = await service.HasPendingSubmissionAsync(1, _sampleUserId);
        var result2 = await service.HasPendingSubmissionAsync(2, _sampleUserId);
        var result3 = await service.HasPendingSubmissionAsync(3, _sampleUserId);

        Assert.That(result1, Is.False);
        Assert.That(result2, Is.True);
        Assert.That(result3, Is.False);
    }
    [Test]
    public async Task CancelPendingAsync_WhenItExists_RemovesPendingSubmission()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
 
        var service = new SubmissionService(repo, _cacheMock.Object);

        Expression<Func<Submission, bool>> predicateToCheck = (s) =>
            s.ChallengeId == 2 && s.UserId == _sampleUserId && s.Status == SubmissionStatus.Pending;

        var pendingBefore = await repo.AnyAsync(predicateToCheck);
        Assert.That(pendingBefore, Is.True);

        await service.CancelPendingAsync(2, _sampleUserId);

        var pendingAfter = await repo.AnyAsync(predicateToCheck);
        Assert.That(pendingAfter, Is.False);
    }

    [Test]
    public async Task CancelPendingAsync_WhenNoPendingSubmissionExists_DoesNothing()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
 
        var service = new SubmissionService(repo, _cacheMock.Object);

        await service.CancelPendingAsync(1, _sampleUserId);

        var exists = await repo.AnyAsync(s =>
            s.ChallengeId == 1 && s.UserId == _sampleUserId && s.Status == SubmissionStatus.Approved);
        Assert.That(exists, Is.True);
    }

    [Test]
    public async Task CreateSubmissionAsync__WhenNoneExists_AddsNewPendingSubmission()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);

        var service = new SubmissionService(repo, _cacheMock.Object);

        var createDto = new SubmissionCreateDto
        {
            ChallengeId = 3,
            SolutionCode = "New Solution",
            Language = SubmissionLanguage.CSharp
        };

        await service.CreateSubmissionAsync(createDto, _sampleUserId);

        var added = await repo.FirstOrDefaultAsync(s =>
            s.ChallengeId == 3 && s.UserId == _sampleUserId && s.Status == SubmissionStatus.Pending);

        Assert.That(added, Is.Not.Null);
        Assert.That(added.SolutionCode, Is.EqualTo("New Solution"));
    }
    [Test]
    public async Task CreateSubmissionAsync_WhenUserIsUnauthenticated_Throws()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);
 
        var service = new SubmissionService(repo, _cacheMock.Object);

        var createDto = new SubmissionCreateDto
        {
            ChallengeId = 3,
            SolutionCode = "New Solution",
            Language = SubmissionLanguage.CSharp
        };

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CreateSubmissionAsync(createDto, null),
            OutputMessages.UnauthenticatedUserSubmissionAttemptMessage
        );
    }
    [Test]
    public async Task CreateSubmissionAsync_WhenPendingSubmissionAlreadyExists_Throws()
    {
        var data = CreateSampleData(_sampleUserId);
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);

        var service = new SubmissionService(repo, _cacheMock.Object);

        var createDto = new SubmissionCreateDto
        {
            ChallengeId = 2,
            SolutionCode = "Another Solution",
            Language = SubmissionLanguage.CSharp
        };

        Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CreateSubmissionAsync(createDto, _sampleUserId),
            OutputMessages.UserAlreadyHasPendingSubmissionMessage
        );
    }
    
    private List<Submission> CreateSampleData(string userId)
    {
        return new List<Submission>
        {
            new Submission
            {
                Id = 1,
                UserId = userId,
                ChallengeId = 1,
                Language = SubmissionLanguage.CSharp,
                Status = SubmissionStatus.Approved,
                Feedback = null,
                SolutionCode = "C# Code",
                SubmittedAt = DateTime.UtcNow,
                Challenge = new Challenge
                {
                    Id = 1,
                    Slug = "test-slug-1",
                    Title = "Test Challenge #1",
                    Description = "desc1",
                    Tags = "tag1"
                }
            },
            new Submission
            {
                Id = 2,
                UserId = userId,
                ChallengeId = 2,
                Language = SubmissionLanguage.Python,
                Status = SubmissionStatus.Pending,
                Feedback = "Nice work",
                SolutionCode = "Python Code",
                SubmittedAt = DateTime.UtcNow.AddDays(-7),
                Challenge = new Challenge
                {
                    Id = 2,
                    Slug = "test-slug-2",
                    Title = "Test Challenge #2",
                    Description = "desc2",
                    Tags = "tag2"
                }
            },
            new Submission
            {
                Id = 3,
                UserId = "other-user",
                ChallengeId = 3,
                Challenge = new Challenge
                {
                    Id = 3,
                    Slug = "challenge-3",
                    Title = "Challenge 3",
                    Description = "desc3",
                    Tags = "tag3"
                },
                Language = SubmissionLanguage.Java,
                Status = SubmissionStatus.Approved,
                Feedback = null,
                SolutionCode = "Code 3",
                SubmittedAt = DateTime.UtcNow
            }
        };
    }
}
