using CodeArena.Common.Exceptions;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Challenge;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Services.QueryModels.Admin;
using CodeArena.Services.Results;
using CodeArena.Web.Areas.Admin.Controllers;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using static CodeArena.Common.ApplicationConstants;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Web.Tests.Admin;

[TestFixture]
public class AdminChallengesControllerTests
{
    private Mock<IAdminChallengeService> _challengeServiceMock = null!;
    private Mock<ILogger<ChallengesController>> _loggerMock = null!;
    private ChallengesController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _challengeServiceMock = new Mock<IAdminChallengeService>();
        _loggerMock = new Mock<ILogger<ChallengesController>>();

        _controller = new ChallengesController(_challengeServiceMock.Object, _loggerMock.Object);

        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );
    }
    [TearDown]
    public void ControllerCleanup()
    {
        _controller.Dispose();
    }
    [Test]
    public async Task Index_ReturnsViewWithChallenges()
    {
        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(It.IsAny<AdminChallengeQuery>()))
            .ReturnsAsync(new PagedResult<ChallengeDisplayDto>(Enumerable.Empty<ChallengeDisplayDto>(), 0));

        var result = await _controller.Index(new()) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<ChallengesIndexViewModel>());
        Assert.That(_controller.ViewData["ActivePage"], Is.EqualTo("Challenges"));
    }
    [Test]
    public async Task CreatePost_ModelInvalid_ReturnsViewWithVm()
    {
        _controller.ModelState.AddModelError("Title", "Required");

        var vm = new CreateChallengeViewModel();
        var result = await _controller.Create(vm) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.EqualTo(vm));
    }

    [Test]
    public async Task CreatePost_Valid_CallsServiceAndRedirects()
    {
        var vm = new CreateChallengeViewModel { Challenge = new CreateChallengeDto() };

        _challengeServiceMock
            .Setup(s => s.CreateChallengeAsync(vm.Challenge))
            .Returns(Task.CompletedTask);

        var result = await _controller.Create(vm) as RedirectToActionResult;

        _challengeServiceMock.Verify(s => s.CreateChallengeAsync(vm.Challenge), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[SuccessTempDataKey], Is.EqualTo(ChallengeCreatedMessage));
    }
    [Test]
    public async Task EditGet_ChallengeExists_ReturnsView()
    {
        var challenge = new ChallengeDisplayDto(1, "", "", "", "Easy", Array.Empty<string>(), 0,false);
        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(1))
            .ReturnsAsync(challenge);

        var result = await _controller.Edit(1) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<CreateChallengeViewModel>());
        Assert.That(_controller.TempData["ChallengeId"], Is.EqualTo(1));
    }

    [Test]
    public async Task EditGet_ChallengeNotFound_Redirects()
    {
        _challengeServiceMock.Setup(s => s.GetChallengeByIdAsync(1))
            .ThrowsAsync(new ChallengeNotFoundException(1));

        var result = await _controller.Edit(1) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task EditPost_ModelInvalid_ReturnsView()
    {
        _controller.ModelState.AddModelError("Title", "Required");
        var vm = new CreateChallengeViewModel { Challenge = new CreateChallengeDto() };

        var result = await _controller.Edit(1, vm) as ViewResult;

        Assert.That(result!.Model, Is.EqualTo(vm));
    }

    [Test]
    public async Task EditPost_Valid_CallsServiceAndRedirects()
    {
        var vm = new CreateChallengeViewModel { Challenge = new CreateChallengeDto() };
        _challengeServiceMock.Setup(s => s.GetChallengeByIdAsync(1))
            .ReturnsAsync(new ChallengeDisplayDto(1, "", "", "", "", Array.Empty<string>(), 0, false));

        _challengeServiceMock.Setup(s => s.UpdateChallengeAsync(It.IsAny<EditChallengeDto>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Edit(1, vm) as RedirectToActionResult;

        _challengeServiceMock.Verify(s => s.UpdateChallengeAsync(It.IsAny<EditChallengeDto>()), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[SuccessTempDataKey], Is.EqualTo(ChallengeUpdatedMessage));
    }
    [Test]
    public async Task Delete_ChallengeDeleted_SetsSuccessMessage()
    {
        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(1))
            .ReturnsAsync(new ChallengeDisplayDto(1, "", "", "", "", Array.Empty<string>(), 0, false));

        _challengeServiceMock.Setup(s => s.DeleteChallengeAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.Delete(1) as RedirectToActionResult;

        Assert.That(_controller.TempData[SuccessTempDataKey], Is.EqualTo(ChallengeUpdatedMessage));
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task Delete_ChallengeNotFound_SetsErrorMessage()
    {
        _challengeServiceMock.Setup(s => s.GetChallengeByIdAsync(1))
            .ThrowsAsync(new ChallengeNotFoundException(1));

        var result = await _controller.Delete(1) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task Restore_ChallengeExists_RestoresAndSetsSuccessMessage()
    {
        var challenge = new ChallengeDisplayDto(1, "Title", "Desc", "Easy", "Code", Array.Empty<string>(), 0, false);

        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(1))
            .ReturnsAsync(challenge);
        _challengeServiceMock
            .Setup(s => s.RestoreChallengeAsync(challenge.Id))
            .Returns(Task.CompletedTask);

        var result = await _controller.Restore(1) as RedirectToActionResult;

        _challengeServiceMock.Verify(s => s.RestoreChallengeAsync(challenge.Id), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[SuccessTempDataKey], Is.EqualTo(ChallengeRestoredMessage));
    }

    [Test]
    public async Task Restore_ChallengeNotFound_SetsErrorMessage()
    {
        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(1))
            .ThrowsAsync(new ChallengeNotFoundException(1));

        var result = await _controller.Restore(1) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }

    [Test]
    public async Task Restore_ChallengeAlreadyActive_SetsErrorMessage()
    {
        var challenge = new ChallengeDisplayDto(1, "Title", "Desc", "Easy", "Code", Array.Empty<string>(), 0, false);

        _challengeServiceMock
            .Setup(s => s.GetChallengeByIdAsync(1))
            .ReturnsAsync(challenge);
        _challengeServiceMock
            .Setup(s => s.RestoreChallengeAsync(challenge.Id))
            .ThrowsAsync(new ChallengeAlreadyActiveException(1));

        var result = await _controller.Restore(1) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));

    }

}