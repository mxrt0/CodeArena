using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.Results;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Tests;

[TestFixture]
public class UserServiceTests
{
    private string _sampleUserId = "user-123";
    private Mock<IMemoryCache> _cacheMock;
    private Mock<IXpService> _xpServiceMock;

    [SetUp]
    public void Setup()
    {
        _xpServiceMock = new Mock<IXpService>();
        _xpServiceMock
            .Setup(xp => xp.GetTotalXpAsync(It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<int>.Ok(0));

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
    public async Task GetUserStatsAsync_ReturnsCorrectCounts()
    {
        var data = CreateSampleData();
        var context = await DbContextFactory.CreateWithDataAsync(data);
        var repo = new SubmissionRepository(context);

        var service = new UserService(repo, _cacheMock.Object, _xpServiceMock.Object);

        var stats = await service.GetUserStatsAsync(_sampleUserId);

        Assert.That(stats.SolvedTotal, Is.EqualTo(2));
        Assert.That(stats.EasySolved, Is.EqualTo(1));
        Assert.That(stats.MediumSolved, Is.EqualTo(1));
        Assert.That(stats.HardSolved, Is.EqualTo(0));
        Assert.That(stats.PendingSubmissions, Is.EqualTo(1));
        Assert.That(stats.RejectedSubmissions, Is.EqualTo(1));
        Assert.That(stats.TotalXp, Is.EqualTo(0));
    }
    [Test]
    public async Task GetUserStatsAsync_WhenNoSubmissionsExist_ReturnsZero()
    {
        var context = DbContextFactory.Create();
        var repo = new SubmissionRepository(context);

        var service = new UserService(repo, _cacheMock.Object, _xpServiceMock.Object);

        var stats = await service.GetUserStatsAsync(_sampleUserId);

        Assert.That(stats.SolvedTotal, Is.EqualTo(0));
        Assert.That(stats.EasySolved, Is.EqualTo(0));
        Assert.That(stats.MediumSolved, Is.EqualTo(0));
        Assert.That(stats.HardSolved, Is.EqualTo(0));
        Assert.That(stats.PendingSubmissions, Is.EqualTo(0));
        Assert.That(stats.RejectedSubmissions, Is.EqualTo(0));
    }
    
    private List<Submission> CreateSampleData()
    {
        return new List<Submission>
        {
            new Submission
            {
                Id = 1,
                UserId = _sampleUserId,
                Challenge = new Challenge { 
                    Id = 1, Difficulty = Difficulty.Easy,
                    Slug = "c1", Title = "Easy Challenge",
                    Description = "desc1", Tags = "tag1"
                },
                ChallengeId = 1,
                Status = SubmissionStatus.Approved,
                Language = SubmissionLanguage.CSharp,
                SolutionCode = "Code 1",
                SubmittedAt = DateTime.UtcNow
            },
            new Submission
            {
                Id = 2,
                UserId = _sampleUserId,
                Challenge = new Challenge { 
                    Id = 2, Difficulty = Difficulty.Medium,
                    Slug = "c2", Title = "Medium Challenge",
                    Description = "desc2", Tags = "tag2"
                },
                ChallengeId = 2,
                Status = SubmissionStatus.Approved,
                Language = SubmissionLanguage.Python,
                SolutionCode = "Code 2",
                SubmittedAt = DateTime.UtcNow
            },
            new Submission
            {
                Id = 3,
                UserId = _sampleUserId,
                Challenge = new Challenge { 
                    Id = 3, Difficulty = Difficulty.Hard,
                    Slug = "c3", Title = "Hard Challenge",
                    Description = "desc3", Tags = "tag3"
                },
                ChallengeId = 3,
                Status = SubmissionStatus.Rejected,
                Language = SubmissionLanguage.Java,
                SolutionCode = "Code 3",
                SubmittedAt = DateTime.UtcNow
            },
            new Submission
            {
                Id = 4,
                UserId = _sampleUserId,
                Challenge = new Challenge { 
                    Id = 4, Difficulty = Difficulty.Easy,
                    Slug = "c4", Title = "Easy Challenge 2",
                    Description = "desc4", Tags = "tag4"
                },
                ChallengeId = 4,
                Status = SubmissionStatus.Pending,
                Language = SubmissionLanguage.CSharp,
                SolutionCode = "Code 4",
                SubmittedAt = DateTime.UtcNow
            },
            new Submission
            {
                Id = 5,
                UserId = "other-user",
                Challenge = new Challenge { 
                    Id = 5, Difficulty = Difficulty.Easy,
                    Slug = "c5", Title = "Other User Challenge",
                    Description = "desc5", Tags = "tag5"
                },
                ChallengeId = 5,
                Status = SubmissionStatus.Approved,
                Language = SubmissionLanguage.Python,
                SolutionCode = "Other Code",
                SubmittedAt = DateTime.UtcNow
            }
        };
    }

}
