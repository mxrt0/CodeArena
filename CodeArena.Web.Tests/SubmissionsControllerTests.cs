using CodeArena.Common;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Services.Results;
using CodeArena.Web.Controllers;
using CodeArena.Web.Models.Challenge;
using CodeArena.Web.Models.Submission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CodeArena.Web.Tests;

[TestFixture]
public class SubmissionsControllerTests
{
    private Mock<ISubmissionService> _submissionServiceMock;
    private Mock<IChallengeService> _challengeServiceMock;
    private Mock<ILogger<SubmissionsController>> _loggerMock;
    private SubmissionsController _controller;

    [SetUp]
    public void Setup()
    {
        _submissionServiceMock = new Mock<ISubmissionService>();
        _challengeServiceMock = new Mock<IChallengeService>();
        _loggerMock = new Mock<ILogger<SubmissionsController>>();

        _controller = new SubmissionsController(
            _submissionServiceMock.Object,
            _challengeServiceMock.Object,
            _loggerMock.Object
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var tempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );
        _controller.TempData = tempData;
    }
    [TearDown]
    public void ControllerCleanup()
    {
        _controller.Dispose();
    }
    [Test]
    public async Task Index_WhenPageIsZero_AdjustsPageToOne()
    {
        _submissionServiceMock
            .Setup(s => s.GetUserSubmissionsAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Enumerable.Empty<SubmissionDisplayDto>(), 0));

        var result = await _controller.Index(0) as ViewResult;

        Assert.That(result, Is.Not.Null);
        var model = result!.Model as SubmissionsIndexViewModel;
        Assert.That(model!.CurrentPage, Is.EqualTo(1));
    }

    [Test]
    public async Task Create_WhenModelStateInvalid_AndChallengeNotFound_ReturnsNotFound()
    {
        _controller.ModelState.AddModelError("SolutionCode", "Required");

        var dto = new SubmissionCreateDto { ChallengeId = 123 };
        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(dto.ChallengeId, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Fail("Not found"));

        var result = await _controller.Create(dto);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Create_WhenModelStateInvalid_AndChallengeExists_ReturnsChallengeView()
    {
        _controller.ModelState.AddModelError("SolutionCode", "Required");

        var dto = new SubmissionCreateDto { ChallengeId = 1, SolutionCode = "code", Language = SubmissionLanguage.CSharp };
        var challengeDto = new ChallengeDisplayDto(
            1, "slug", "Test", "Desc", "Easy", Array.Empty<string>(), 0, false);

        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(dto.ChallengeId, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(ServiceResult<ChallengeDisplayDto>.Ok(challengeDto));

        var result = await _controller.Create(dto) as ViewResult;

        Assert.That(result!.ViewName, Is.EqualTo("~/Views/Challenges/Details.cshtml"));
        var model = result.Model as ChallengeDetailsViewModel;
        Assert.That(model!.SolutionCode, Is.EqualTo(dto.SolutionCode));
        Assert.That(model.Challenge.Id, Is.EqualTo(1));
    }

    [Test]
    public async Task Create_WhenModelStateValid_CreatesSubmission_AndRedirects()
    {
        _submissionServiceMock
        .Setup(s => s.CreateSubmissionAsync(It.IsAny<SubmissionCreateDto>(), It.IsAny<ClaimsPrincipal>()))
        .Returns(Task.CompletedTask);

        var dto = new SubmissionCreateDto { ChallengeId = 1, SolutionCode = "code", Language = SubmissionLanguage.CSharp };

        var result = await _controller.Create(dto) as RedirectToActionResult;

        _submissionServiceMock.Verify(s => s.CreateSubmissionAsync(dto, It.IsAny<ClaimsPrincipal>()), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task Cancel_SetsTempData_AndRedirectsCorrectly()
    {
        _submissionServiceMock
        .Setup(s => s.CancelPendingAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
        .Returns(Task.CompletedTask);

        var result = await _controller.Cancel(1, redirectToSubmissions: true) as RedirectToActionResult;
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[ApplicationConstants.InfoTempDataKey], Is.EqualTo(OutputMessages.SubmissionCancelledMessage));

        result = await _controller.Cancel(1, redirectToSubmissions: false) as RedirectToActionResult;
        Assert.That(result!.ActionName, Is.EqualTo("Details"));
        Assert.That(result.ControllerName, Is.EqualTo("Challenges"));
    }

    [Test]
    public async Task Details_WhenIdInvalid_ReturnsBadRequest()
    {
        var result = await _controller.Details(0);
        Assert.That(result, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Details_WhenServiceFails_ReturnsNotFound_AndLogs()
    {
        _submissionServiceMock
            .Setup(s => s.GetUserSubmissionsAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Enumerable.Empty<SubmissionDisplayDto>(), 0));

        _submissionServiceMock
            .Setup(s => s.GetSubmissionDetailsAsync(It.IsAny<int>(), It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(ServiceResult<SubmissionDetailsDto>.Fail("Not found"));

        var result = await _controller.Details(1) as NotFoundResult;

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
    public async Task Details_WhenServiceSucceeds_ReturnsViewWithModel()
    {
        var dto = new SubmissionDetailsDto(1, "", "", "", "", "", "", DateTime.UtcNow);
        _submissionServiceMock
            .Setup(s => s.GetSubmissionDetailsAsync(1, It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(ServiceResult<SubmissionDetailsDto>.Ok(dto));

        var result = await _controller.Details(1) as ViewResult;

        Assert.That(result, Is.Not.Null);
        var model = result!.Model as SubmissionDetailsViewModel;
        Assert.That(model!.Submission.SubmissionId, Is.EqualTo(1));
    }
}