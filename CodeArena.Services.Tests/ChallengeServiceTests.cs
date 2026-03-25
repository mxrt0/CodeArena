using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CodeArena.Services.Tests;

[TestFixture]
public class Tests
{
    private UserManager<ApplicationUser> _userManager;
    private Challenge _exampleChallenge;
    [OneTimeSetUp]
    public void Setup()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
        var normalizer = new Mock<ILookupNormalizer>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();
        var errorDescriber = new Mock<IdentityErrorDescriber>();
        var serviceProvider = new Mock<IServiceProvider>();

        _userManager = new UserManager<ApplicationUser>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                normalizer.Object,
                errorDescriber.Object,
                serviceProvider.Object,
                logger.Object
            );
        _exampleChallenge = new Challenge
        {
            Id = 1,
            Title = "Sum Two Numbers",
            Difficulty = Difficulty.Easy,
            Tags = "math",
            Description = "Write a function that takes two numbers and returns their sum. Example: Input: 3, 5 → Output: 8.",
            Slug = "sum-two-numbers"
        };
    }
    [OneTimeTearDown]
    public void Dispose()
    {
        _userManager.Dispose();
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenNonExistentId_ReturnsFailResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();
        var submissionServiceMock = new Mock<ISubmissionService>();

        challengeRepositoryMock
            .Setup(cr => cr.GetByIdAsync(It.IsAny<int>(), false))
            .ReturnsAsync((Challenge?)null);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _userManager,
            submissionServiceMock.Object
            );

        var result = await service.GetChallengeByIdAsync(123);

        Assert.That(result.Success, Is.EqualTo(false));
    }

    [Test]
    public async Task GetChallengeByIdAsync_WhenExistentId_ReturnsSuccessResult()
    {
        var challengeRepositoryMock = new Mock<IChallengeRepository>();
        var submissionServiceMock = new Mock<ISubmissionService>();
        int id = 1;

        challengeRepositoryMock
            .Setup(cr => cr.GetByIdAsync(id, false))
            .ReturnsAsync(_exampleChallenge);

        var service = new ChallengeService(
            challengeRepositoryMock.Object,
            _userManager,
            submissionServiceMock.Object
            );

        var result = await service.GetChallengeByIdAsync(id);

        Assert.That(result.Success, Is.EqualTo(true));
    }
}