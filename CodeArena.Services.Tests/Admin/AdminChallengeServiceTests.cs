using CodeArena.Common.Exceptions;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.DTOs.Admin;
using CodeArena.Services.Core.Admin;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        var service = new AdminChallengeService(repo);

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
        var service = new AdminChallengeService(repo);

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
        var service = new AdminChallengeService(repo);

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
        var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

            await service.DeleteChallengeAsync(1);
        });
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenExists_ReturnsDto()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);
        var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

            await service.GetChallengeByIdAsync(123);
        });
    }

    [Test]
    public async Task GetChallengesAsync_WhenCalled_ReturnsAllChallenges()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);
        var service = new AdminChallengeService(repo);

        var challenges = await service.GetChallengesAsync();

        Assert.That(challenges.Count(), Is.EqualTo(_sampleData.Count));
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
        var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

            await service.RestoreChallengeAsync(1);
        });
    }

    [Test]
    public async Task UpdateChallengeAsync_WhenExists_UpdatesChallenge()
    {
        var context = await DbContextFactory.CreateWithDataAsync(_sampleData);
        var repo = new ChallengeRepository(context);
        var service = new AdminChallengeService(repo);

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
            var service = new AdminChallengeService(repo);

            await service.UpdateChallengeAsync(new EditChallengeDto( Id: 123, "", "", Difficulty.Easy, "" ));
        });
    }
}