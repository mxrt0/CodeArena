using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.QueryModels;
using CodeArena.Services.Tests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Data;
using System.Security.Claims;

namespace CodeArena.Services.Tests;

[TestFixture]
public class ChallengeServiceTests
{
    private Challenge _exampleChallenge;
    private List<Challenge> _sampleData;
    private Mock<IMemoryCache> _cacheMock;
    private Mock<ISubmissionService> _submissionServiceMock;
    [SetUp]
    public void Setup()
    {
        _submissionServiceMock = new Mock<ISubmissionService>();

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

        _exampleChallenge = new Challenge
        {
            Id = 1,
            Title = "Sum Two Numbers",
            Difficulty = Difficulty.Easy,
            Tags = "math",
            Description = "Write a function that takes two numbers and returns their sum. Example: Input: 3, 5 → Output: 8.",
            Slug = "sum-two-numbers"
        };
        _sampleData = new List<Challenge>
        {
            new Challenge { Id = 1, Title = "A",
                Difficulty = Difficulty.Easy, Tags = "",
                Submissions = new List<Submission>(),
            Description = "desc1", Slug = "slug1"
            },
            new Challenge { Id = 2, Title = "B", Difficulty = Difficulty.Hard,
                Tags = "", Submissions = new List<Submission>(),
            Description = "desc2", Slug = "slug2"}
        };
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenNonExistentId_ReturnsFailResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();

        challengeRepositoryMock
            .Setup(cr => cr.GetByIdAsync(It.IsAny<int>(), false))
            .ReturnsAsync((Challenge?)null);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _submissionServiceMock.Object,
            _cacheMock.Object
            );

        var result = await service.GetChallengeByIdAsync(123, null);

        Assert.That(result.Success, Is.EqualTo(false));
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenExistentId_ReturnsSuccessResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();
        int id = 1;

        challengeRepositoryMock
            .Setup(cr => cr.GetByIdAsync(id, false))
            .ReturnsAsync(_exampleChallenge);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _submissionServiceMock.Object,
            _cacheMock.Object
            );

        var result = await service.GetChallengeByIdAsync(id, null);

        Assert.That(result.Success, Is.True);
    }
    [Test]
    public async Task GetChallengeBySlugAsync_WhenNonExistentSlug_ReturnsFailResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();

        challengeRepositoryMock
            .Setup(cr => cr.GetBySlugAsync(It.IsAny<string>()))
            .ReturnsAsync((Challenge?)null);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _submissionServiceMock.Object,
            _cacheMock.Object
            );

        var result = await service.GetChallengeBySlugAsync("slug", null);

        Assert.That(result.Success, Is.False);
    }

    [Test]
    public async Task GetChallengeBySlugAsync_WhenExistentSlug_ReturnsSuccessResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();
        string slug = "validSlug";

        challengeRepositoryMock
            .Setup(cr => cr.GetBySlugAsync(slug))
            .ReturnsAsync(_exampleChallenge);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _submissionServiceMock.Object,
            _cacheMock.Object
            );

        var result = await service.GetChallengeBySlugAsync(slug, null);

        Assert.That(result.Success, Is.EqualTo(true));
    }
    [Test]
    public async Task GetChallengesAsync_NoFilters_ReturnsAllChallenges()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);

        var repo = new ChallengeRepository(context);

        _submissionServiceMock
            .Setup(s => s.HasPendingSubmissionAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        _submissionServiceMock
            .Setup(s => s.HasApprovedSubmissionAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var service = new ChallengeService(
            repo,
            _submissionServiceMock.Object, _cacheMock.Object
        );

        var result = await service.GetChallengesAsync(new(), null);

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Items.Count(), Is.EqualTo(2));
    }
    [Test]
    public async Task GetChallengesAsync_WithDifficultyFilter_ReturnsFiltered()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);

        var repo = new ChallengeRepository(context);

        var service = new ChallengeService(repo,
            _submissionServiceMock.Object, _cacheMock.Object);

        var result = await service.GetChallengesAsync(
            new ChallengeQuery
            {
                Difficulty = Difficulty.Easy
            },
            "userId"
        );

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Items.First().Difficulty, Is.EqualTo(Difficulty.Easy.ToString()));
    }
    [Test]
    public async Task GetChallengesAsync_WithPagination_ReturnsCorrectPage()
    {
        var context = await DbContextFactory.CreateWithDataAsync(Enumerable.Range(1, 20)
            .Select(i => new Challenge
            {
                Id = i,
                Title = i.ToString(),
                Tags = "",
                Difficulty = Difficulty.Easy,
                Submissions = new List<Submission>(),
                Description = $"desc{i}",
                Slug = $"slug{i}"
            }));

        var repo = new ChallengeRepository(context);

        var service = new ChallengeService(repo,
            _submissionServiceMock.Object, _cacheMock.Object);

        var result = await service.GetChallengesAsync(
            new ChallengeQuery
        {
            Page = 2,
            PageSize = 5
        }, "userId");

        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.Items.Count(), Is.EqualTo(5));
    }
    [Test]
    public async Task GetChallengesAsync_SetsSubmissionFlagsCorrectly()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);

        var repo = new ChallengeRepository(context);

        _submissionServiceMock
            .Setup(s => s.HasPendingSubmissionAsync(1, It.IsAny<string>()))
            .ReturnsAsync(true);

        _submissionServiceMock
            .Setup(s => s.HasApprovedSubmissionAsync(1, It.IsAny<string>()))
            .ReturnsAsync(true);

        var service = new ChallengeService(repo,
            _submissionServiceMock.Object, _cacheMock.Object);

        var result = await service.GetChallengesAsync(new(), "test");

        var dto = result.Items.First();

        Assert.That(dto.HasPendingSubmission, Is.True);
        Assert.That(dto.IsSolved, Is.True);
    }
}