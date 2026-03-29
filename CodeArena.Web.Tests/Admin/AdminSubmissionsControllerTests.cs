using CodeArena.Common;
using CodeArena.Common.Exceptions;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Submission;
using CodeArena.Web.Areas.Admin.Controllers;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using static CodeArena.Common.ApplicationConstants;
using static CodeArena.Common.OutputMessages;
namespace CodeArena.Web.Tests.Admin;

[TestFixture]
public class AdminSubmissionsControllerTests
{
    private Mock<IAdminSubmissionService> _submissionServiceMock = null!;
    private Mock<ILogger<SubmissionsController>> _loggerMock = null!;
    private SubmissionsController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _submissionServiceMock = new Mock<IAdminSubmissionService>();
        _loggerMock = new Mock<ILogger<SubmissionsController>>();

        _controller = new SubmissionsController(
            _submissionServiceMock.Object,
            _loggerMock.Object
        );

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
    public async Task Index_ReturnsViewWithCorrectPagination()
    {

        _submissionServiceMock
            .Setup(s => s.GetPendingSubmissionsAsync(1, 5))
            .ReturnsAsync((Enumerable.Empty<SubmissionDisplayDto>(), 10));

        var result = await _controller.Index(1) as ViewResult;

        Assert.That(result, Is.Not.Null);

        var model = result!.Model as SubmissionsIndexViewModel;
        Assert.That(model, Is.Not.Null);
        Assert.That(model!.CurrentPage, Is.EqualTo(1));
        Assert.That(model.TotalPages, Is.EqualTo(2));

        Assert.That(_controller.ViewData["ActivePage"], Is.EqualTo("Submissions"));
    }
    [Test]
    public async Task Review_SubmissionExists_ReturnsView()
    {

        _submissionServiceMock
            .Setup(s => s.GetSubmissionForReviewAsync(1))
            .ReturnsAsync(new AdminSubmissionReviewDto(1, "", "", "Python", "Easy", "code", DateTime.UtcNow));

        var result = await _controller.Review(1) as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<SubmissionReviewViewModel>());
    }
    [Test]
    public async Task Review_NotFound_RedirectsToIndex()
    {
        _submissionServiceMock
            .Setup(s => s.GetSubmissionForReviewAsync(1))
            .ThrowsAsync(new SubmissionNotFoundException(1));

        var result = await _controller.Review(1) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task Approve_Valid_CallsServiceAndSetsSuccessMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "ok"
        };

        _submissionServiceMock
            .Setup(s => s.ApproveAsync(1, "ok"))
            .Returns(Task.CompletedTask);

        var result = await _controller.Approve(vm) as RedirectToActionResult;

        _submissionServiceMock.Verify(s => s.ApproveAsync(1, "ok"), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[SuccessTempDataKey],
            Is.EqualTo(SubmissionApprovedMessage));
    }
    [Test]
    public async Task Approve_AlreadyApproved_SetsCorrectErrorMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "ok"
        };

        _submissionServiceMock
            .Setup(s => s.ApproveAsync(1, "ok"))
            .ThrowsAsync(new SubmissionAlreadyApprovedException(1));

        var result = await _controller.Approve(vm) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[ErrorTempDataKey], Is.EqualTo(Admin_SubmissionAlreadyApprovedMessage));
    }
    [Test]
    public async Task Reject_Valid_CallsServiceAndSetsSuccessMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "bad"
        };

        _submissionServiceMock
            .Setup(s => s.RejectAsync(1, "bad"))
            .Returns(Task.CompletedTask);

        var result = await _controller.Reject(vm) as RedirectToActionResult;

        _submissionServiceMock.Verify(s => s.RejectAsync(1, "bad"), Times.Once);
        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[SuccessTempDataKey], Is.EqualTo(SubmissionRejectedMessage));
    }
    [Test]
    public async Task Reject_AlreadyRejected_SetsCorrectErrorMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "bad"
        };

        _submissionServiceMock
            .Setup(s => s.RejectAsync(1, "bad"))
            .ThrowsAsync(new SubmissionAlreadyRejectedException(1));

        var result = await _controller.Reject(vm) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
        Assert.That(_controller.TempData[ErrorTempDataKey], Is.EqualTo(Admin_SubmissionAlreadyRejectedMessage));
    }
    [Test]
    public async Task Approve_NotFound_RedirectsToIndex()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "ok"
        };

        _submissionServiceMock
            .Setup(s => s.ApproveAsync(1, "ok"))
            .ThrowsAsync(new SubmissionNotFoundException(1));

        var result = await _controller.Approve(vm) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task Approve_AlreadyRejected_SetsErrorMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "ok"
        };

        _submissionServiceMock
            .Setup(s => s.ApproveAsync(1, "ok"))
            .ThrowsAsync(new SubmissionAlreadyRejectedException(1));

        var result = await _controller.Approve(vm) as RedirectToActionResult;

        Assert.That(_controller.TempData[ErrorTempDataKey], Is.EqualTo(Admin_SubmissionAlreadyRejectedMessage));
    }
    [Test]
    public async Task Reject_NotFound_RedirectsToIndex()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "bad"
        };

        _submissionServiceMock
            .Setup(s => s.RejectAsync(1, "bad"))
            .ThrowsAsync(new SubmissionNotFoundException(1));

        var result = await _controller.Reject(vm) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));
    }
    [Test]
    public async Task Reject_AlreadyApproved_SetsErrorMessage()
    {
        var vm = new SubmissionReviewViewModel
        {
            Submission = new(1, "", "", "Python", "Easy", "code", DateTime.UtcNow),
            SubmissionFeedback = "bad"
        };

        _submissionServiceMock
            .Setup(s => s.RejectAsync(1, "bad"))
            .ThrowsAsync(new SubmissionAlreadyApprovedException(1));

        var result = await _controller.Reject(vm) as RedirectToActionResult;

        Assert.That(_controller.TempData[ErrorTempDataKey], Is.EqualTo(Admin_SubmissionAlreadyApprovedMessage));
    }

}