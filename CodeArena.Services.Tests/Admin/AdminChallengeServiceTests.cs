using CodeArena.Common.Exceptions;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Migrations;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core;
using CodeArena.Services.Core.Admin;
using CodeArena.Services.DTOs.Admin;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.QueryModels;
using CodeArena.Services.QueryModels.Admin;
using CodeArena.Services.Tests.Infrastructure;
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
public class AdminChallengeServiceTests
{
    private List<Challenge> _sampleData;
    private Challenge _exampleChallenge;
    private Mock<IMemoryCache> _cacheMock;
    [SetUp]
    public void CacheSetup()
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
    [OneTimeSetUp]
    public void Setup()
    {
        _exampleChallenge = new Challenge
        {
            Id = 1,
            Title = "Test Challenge",
            Description = "Desc",
            Difficulty = Difficulty.Medium,
            Tags = "tag1,tag2",
            Slug = "test-challenge",
            Submissions = new List<Submission>()
        };

        _sampleData = new List<Challenge>
        {
            _exampleChallenge,
            new Challenge { Id = 2, Title = "Another", Description = "Desc2",
                Difficulty = Difficulty.Hard, Tags = "", Slug = "another",
                Submissions = new List<Submission>() }
        };
    }

    [Test]
    public async Task CreateChallengeAsync_WhenCalled_AddsChallengeWithUniqueSlug()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);
 
        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var dto = new CreateChallengeDto
        {
            Title = "New Challenge",
            Description = "Desc",
            Difficulty = Difficulty.Easy,
            Tags = "t1"
        };

        await service.CreateChallengeAsync(dto);

        var created = await context.Challenges.FirstOrDefaultAsync(c => c.Title == "New Challenge");
        Assert.That(created, Is.Not.Null);
        Assert.That(created.Slug, Is.Not.Empty);
        Assert.That(created.Description, Is.EqualTo(dto.Description));
        Assert.That(created.Difficulty, Is.EqualTo(dto.Difficulty));
        Assert.That(created.Tags, Is.EqualTo(dto.Tags));
    }
    [Test]
    public async Task CreateChallengeAsync_WhenTagsNull_SetsEmptyString()
    {
        var context = DbContextFactory.Create();
        var repo = new ChallengeRepository(context);
 
        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var dto = new CreateChallengeDto
        {
            Title = "Test",
            Description = "Desc",
            Difficulty = Difficulty.Easy,
            Tags = null
        };

        await service.CreateChallengeAsync(dto);

        var created = context.Challenges.First();
        Assert.That(created.Tags, Is.EqualTo(string.Empty));
    }
    [Test]
    public async Task CreateChallengeAsync_WhenSlugExists_GeneratesUniqueSlug()
    {
        var existing = new Challenge
        {
            Id = 1,
            Title = "Test",
            Slug = "test",
            Description = "d",
            Difficulty = Difficulty.Easy,
            Tags = ""
        };

        var context = await DbContextFactory.CreateWithDataAsync(new List<Challenge> { existing });
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var dto = new CreateChallengeDto { 
            Title = "Test",
            Description = "New",
            Difficulty = Difficulty.Easy,
            Tags = "t"
        };

        await service.CreateChallengeAsync(dto);

        var created = context.Challenges
            .First(c => c.Id != 1);

        Assert.That(created.Slug, Is.Not.EqualTo("test"));
    }

    [Test]
    public async Task DeleteChallengeAsync_WhenChallengeExistsAndNotDeleted_DeletesChallenge()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);
 
        var service = new AdminChallengeService(repo, _cacheMock.Object);

        await service.DeleteChallengeAsync(1);

        var deleted = await context.Challenges.FindAsync(1);
        Assert.That(deleted?.IsDeleted, Is.True);
    }

    [Test]
    public void DeleteChallengeAsync_WhenChallengeDoesNotExist_ThrowsNotFound()
    {
        Assert.ThrowsAsync<ChallengeNotFoundException>(async () =>
        {
            var context = DbContextFactory.Create();
            var repo = new ChallengeRepository(context);
 
            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.DeleteChallengeAsync(123);
        });
    }

    [Test]
    public void DeleteChallengeAsync_WhenChallengeAlreadyDeleted_ThrowsAlreadyDeleted()
    {
        Assert.ThrowsAsync<ChallengeAlreadyDeletedException>(async () =>
        {
            var deletedChallenge = new Challenge { 
                Id = 1, IsDeleted = true,
                Title = "Test Challenge",
                Description = "Desc",
                Difficulty = Difficulty.Medium,
                Tags = "tag1,tag2",
                Slug = "test-challenge",
            };
            var context = await DbContextFactory.CreateWithDataAsync(new List<Challenge> { deletedChallenge });
            var repo = new ChallengeRepository(context);

            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.DeleteChallengeAsync(1);
        });
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenExists_ReturnsDto()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var dto = await service.GetChallengeByIdAsync(1);

        Assert.That(dto.Id, Is.EqualTo(1));
        Assert.That(dto.Tags, Is.EquivalentTo(new[] { "tag1", "tag2" }));
    }

    [Test]
    public void GetChallengeByIdAsync_WhenNotExists_ThrowsNotFound()
    {
        Assert.ThrowsAsync<ChallengeNotFoundException>(async () =>
        {
            var context = DbContextFactory.Create();
            var repo = new ChallengeRepository(context);
 
            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.GetChallengeByIdAsync(123);
        });
    }

    [Test]
    public async Task GetChallengesAsync_WhenCalled_ReturnsAllChallenges()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var result = await service.GetChallengesAsync(new());

        Assert.That(result.Items.Count(), Is.EqualTo(_sampleData.Count));
    }
    [Test]
    public async Task GetChallengesAsync_NoFilters_ReturnsAllChallenges()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);   

        var result = await service.GetChallengesAsync(new());

        Assert.That(result.TotalCount, Is.EqualTo(2));
        Assert.That(result.Items.Count(), Is.EqualTo(2));
    }
    [Test]
    public async Task GetChallengesAsync_WithDifficultyFilter_ReturnsFiltered()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);

        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var result = await service.GetChallengesAsync(
            new AdminChallengeQuery
            {
                Difficulty = Difficulty.Medium
            }
        );

        Assert.That(result.TotalCount, Is.EqualTo(1));
        Assert.That(result.Items.First().Difficulty, Is.EqualTo(Difficulty.Medium.ToString()));
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

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var result = await service.GetChallengesAsync(
            new AdminChallengeQuery
            {
                Page = 2,
                PageSize = 5
            }
        );

        Assert.That(result.TotalCount, Is.EqualTo(20));
        Assert.That(result.Items.Count(), Is.EqualTo(5));
    }
    [Test]
    public async Task RestoreChallengeAsync_WhenDeleted_RestoresChallenge()
    {
        var deleted = new Challenge
        {
            Id = 1,
            IsDeleted = true,
            Title = "Test Challenge",
            Description = "Desc",
            Difficulty = Difficulty.Medium,
            Tags = "tag1,tag2",
            Slug = "test-challenge",
        };
        var context = await DbContextFactory.CreateWithDataAsync(new List<Challenge> { deleted });
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        await service.RestoreChallengeAsync(1);

        var restored = await context.Challenges.FindAsync(1);
        Assert.That(restored?.IsDeleted, Is.False);
    }

    [Test]
    public void RestoreChallengeAsync_WhenNotFound_ThrowsNotFound()
    {
        Assert.ThrowsAsync<ChallengeNotFoundException>(async () =>
        {
            var context = DbContextFactory.Create();
            var repo = new ChallengeRepository(context);

            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.RestoreChallengeAsync(1);
        });
    }

    [Test]
    public void RestoreChallengeAsync_WhenAlreadyActive_ThrowsAlreadyActive()
    {
        Assert.ThrowsAsync<ChallengeAlreadyActiveException>(async () =>
        {
            var active = new Challenge {
                Id = 1, IsDeleted = false,
                Title = "Test Challenge",
                Description = "Desc",
                Difficulty = Difficulty.Medium,
                Tags = "tag1,tag2",
                Slug = "test-challenge",
            };
            var context = await DbContextFactory.CreateWithDataAsync(new List<Challenge> { active });
            var repo = new ChallengeRepository(context);
 
            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.RestoreChallengeAsync(1);
        });
    }

    [Test]
    public async Task UpdateChallengeAsync_WhenExists_UpdatesChallenge()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);

        var service = new AdminChallengeService(repo, _cacheMock.Object);

        var editDto = new EditChallengeDto
        (
            Id: 1,
            Title: "Updated",
            Description: "UpdatedDesc",
            Difficulty: Difficulty.Hard,
            Tags: "newtag"
        );

        await service.UpdateChallengeAsync(editDto);

        var updated = await context.Challenges.FindAsync(1);
        Assert.That(updated?.Title, Is.EqualTo("Updated"));
        Assert.That(updated.Description, Is.EqualTo("UpdatedDesc"));
        Assert.That(updated.Difficulty, Is.EqualTo(Difficulty.Hard));
        Assert.That(updated.Tags, Is.EqualTo("newtag"));
    }

    [Test]
    public void UpdateChallengeAsync_WhenNotFound_ThrowsNotFound()
    {
        Assert.ThrowsAsync<ChallengeNotFoundException>(async () =>
        {
            var context = await DbContextFactory.CreateWithDataAsync(new List<Challenge>());
            var repo = new ChallengeRepository(context);
 
            var service = new AdminChallengeService(repo, _cacheMock.Object);

            await service.UpdateChallengeAsync(new EditChallengeDto( Id: 123, "", "", Difficulty.Easy, "" ));
        });
    }
}