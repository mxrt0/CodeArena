using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Web.Areas.Admin.Models;
using static CodeArena.Common.OutputMessages;
using static CodeArena.Common.ApplicationConstants;
using Microsoft.AspNetCore.Mvc;
using CodeArena.Common.Exceptions;
using AspNetCoreGeneratedDocument;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class SubmissionsController : BaseAdminController
{
    const int PageSize = 5;
    private readonly IAdminSubmissionService _submissionService;
    private readonly ILogger<SubmissionsController> _logger;
    public SubmissionsController(IAdminSubmissionService submissionService,
        ILogger<SubmissionsController> logger)
    {
        _submissionService = submissionService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var (submissions, count) = await _submissionService.GetPendingSubmissionsAsync(
            Math.Max(1, page),
            PageSize
        );

        var vm = new SubmissionsIndexViewModel
        {
            Submissions = submissions,
            CurrentPage = Math.Max(1, page),
            TotalPages = (int)Math.Ceiling(count / (double)PageSize)
        };

        ViewData["ActivePage"] = "Submissions";
        return View(vm);
    }

    public async Task<IActionResult> Review(int id)
    {
        try
        {
            var submission = await _submissionService.GetSubmissionForReviewAsync(id);

            var vm = new SubmissionReviewViewModel
            {
                Submission = submission
            };
            return View(vm);
        }
        catch (SubmissionNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Approve(SubmissionReviewViewModel vm) 
    {
        try
        {
            await _submissionService.ApproveAsync(vm.Submission.SubmissionId, vm.SubmissionFeedback);
            TempData[SuccessTempDataKey] = SubmissionApprovedMessage;
        }
        catch (SubmissionNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
        }
        catch (SubmissionAlreadyApprovedException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = Admin_SubmissionAlreadyApprovedMessage;
        }
        catch (SubmissionAlreadyRejectedException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = Admin_SubmissionAlreadyRejectedMessage;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(SubmissionReviewViewModel vm)
    {
        try
        {
            await _submissionService.RejectAsync(vm.Submission.SubmissionId, vm.SubmissionFeedback);
            TempData[SuccessTempDataKey] = SubmissionRejectedMessage;
        }
        catch (SubmissionNotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = ex.Message;
        }
        catch (SubmissionAlreadyApprovedException ex)
        {
            _logger.LogWarning(ex.Message);
            TempData[ErrorTempDataKey] = Admin_SubmissionAlreadyApprovedMessage;
        }
        catch (SubmissionAlreadyRejectedException ex)
        {
            _logger.LogWarning(ex.Message); 
            TempData[ErrorTempDataKey] = Admin_SubmissionAlreadyRejectedMessage;
        }

        return RedirectToAction(nameof(Index));
    }
}
