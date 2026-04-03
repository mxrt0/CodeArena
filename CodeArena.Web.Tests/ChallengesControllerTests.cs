using CodeArena.Common.Enums;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Controllers;
using CodeArena.Web.Models.Challenge;
using CodeArena.Services.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.Results;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.QueryModels;

namespace CodeArena.Web.Tests;

[TestFixture]
public class ChallengesControllerTests
{
    private Mock<IChallengeService> _challengeServiceMock;
    private Mock<ISubmissionService> _submissionServiceMock;
    private Mock<ILogger<ChallengesController>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _challengeServiceMock = new Mock<IChallengeService>();
        _submissionServiceMock = new Mock<ISubmissionService>();
        _loggerMock = new Mock<ILogger<ChallengesController>>();
    }

    private ChallengesController CreateController(bool isAuthenticated = false)
    {
        var controller = new ChallengesController(
            _challengeServiceMock.Object,
            _submissionServiceMock.Object,
            _loggerMock.Object
        );

        var user = isAuthenticated
            ? new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user-1")
                }, "mock"))
            : new ClaimsPrincipal(new ClaimsIdentity());

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    [Test]
    public async Task Index_WhenCalled_ReturnsViewWithCorrectModel()
    {
        var controller = CreateController();

        var challenges = new List<ChallengeDisplayDto>
        {
            new ChallengeDisplayDto(1, "slug-1", "Title 1", "Desc 1", "Easy", new[] {"tag1"}, 0, false),
            new ChallengeDisplayDto(2, "slug-2", "Title 2", "Desc 2", "Medium", new[] {"tag2"}, 2, false)
        };

        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(
                It.IsAny<ChallengeQuery>(),
                It.IsAny<string>()))
            .ReturnsAsync(new PagedResult<ChallengeDisplayDto>(challenges, 24));

        var result = await controller.Index(new ChallengeQuery
        {
            Page = 2
        });

        var view = result as ViewResult;
        var model = view?.Model as ChallengeIndexViewModel;

        Assert.That(view, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model.CurrentPage, Is.EqualTo(2));
        Assert.That(model.TotalPages, Is.EqualTo(2));
        Assert.That(model.Challenges.Count, Is.EqualTo(challenges.Count));
    }

    [Test]
    public async Task Index_WhenUserNotAuthenticated_PassesNullUserToService()
    {
        var controller = CreateController(false);

        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(
                It.IsAny<ChallengeQuery>(),
                It.IsAny<string>()))
            .ReturnsAsync(new PagedResult<ChallengeDisplayDto>(new List<ChallengeDisplayDto>(), 0));

        await controller.Index(new());

        _challengeServiceMock.VerifyAll();
    }

    [Test]
    public async Task Index_WhenUserAuthenticated_PassesUserToService()
    {
        var controller = CreateController(true);

        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(
                It.IsAny<ChallengeQuery>(),
                It.IsAny<string>()))
            .ReturnsAsync(new PagedResult<ChallengeDisplayDto>(new List<ChallengeDisplayDto>(), 0));

        await controller.Index(new());

        _challengeServiceMock.Verify(s => s.GetChallengesAsync(
            It.IsAny<ChallengeQuery>(),
            It.IsAny<string>()
        ), Times.Once);
    }

    [Test]
    public async Task Details_WhenSlugIsNull_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.Details(null!);

        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Details_WhenSlugIsEmpty_ReturnsBadRequest()
    {
        var controller = CreateController();

        var result = await controller.Details("");

        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Details_WhenServiceFails_ReturnsNotFound()
    {
        var controller = CreateController();

        _challengeServiceMock
            .Setup(s => s.GetChallengeBySlugAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Fail("not found"));

        var result = await controller.Details("slug");

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Details_WhenSuccess_ReturnsViewWithModel()
    {
        var controller = CreateController();

        var dto = new ChallengeDisplayDto(1, "slug", "Title", "Desc", "Easy", new[] { "tag" }, 0, false);

        _challengeServiceMock
            .Setup(s => s.GetChallengeBySlugAsync(
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Ok(dto));

        var result = await controller.Details("slug");

        var view = result as ViewResult;
        var model = view?.Model as ChallengeDetailsViewModel;

        Assert.That(view, Is.Not.Null);
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Challenge, Is.EqualTo(dto));
    }
    [Test]
    public async Task Index_WhenPageIsZero_AdjustsPageToOne()
    {
        var controller = CreateController();
        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(It.IsAny<ChallengeQuery>(), It.IsAny<string>()))
            .ReturnsAsync(new PagedResult<ChallengeDisplayDto>(Enumerable.Empty<ChallengeDisplayDto>(), 0));

        var vm = new ChallengeIndexViewModel();
        var result = await controller.Index(new ChallengeQuery
        {
            Page = 0
        }) as ViewResult;

        Assert.That(result, Is.Not.Null);
        var model = result!.Model as ChallengeIndexViewModel;
        Assert.That(model!.CurrentPage, Is.EqualTo(1));
    }

    [Test]
    public async Task Details_WhenSlugIsNullOrWhitespace_ReturnsBadRequest()
    {
        var controller = CreateController();
        var result = await controller.Details("  ");
        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Details_WhenServiceFails_ReturnsNotFound_AndLogs()
    {
        var controller = CreateController();
        _challengeServiceMock
            .Setup(s => s.GetChallengeBySlugAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Fail("Not found"));

        var result = await controller.Details("slug") as NotFoundResult;

        Assert.That(result, Is.Not.Null);
        _loggerMock.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Not found")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task Details_WhenChallengeHasFlags_SetsViewModelCorrectly()
    {
        var controller = CreateController();

        var dto = new ChallengeDisplayDto(
            Id: 1,
            Slug: "test",
            Title: "Test",
            Description: "Desc",
            Difficulty: "Easy",
            Tags: new[] { "tag1", "tag2" },
            SubmissionCount: 5,
            IsDeleted: false
        )
        { HasPendingSubmission = true, IsSolved = true };

        _challengeServiceMock
            .Setup(s => s.GetChallengeBySlugAsync("test", It.IsAny<string>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Ok(dto));

        var result = await controller.Details("test") as ViewResult;

        Assert.That(result, Is.Not.Null);
        var model = result!.Model as ChallengeDetailsViewModel;
        Assert.That(model!.Challenge.HasPendingSubmission, Is.True);
        Assert.That(model.Challenge.IsSolved, Is.True);
    }
}