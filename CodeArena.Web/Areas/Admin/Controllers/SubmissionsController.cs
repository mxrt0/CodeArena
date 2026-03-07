using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Areas.Admin.Controllers;

public class SubmissionsController : BaseAdminController
{
    private readonly IAdminSubmissionService _submissionService;
    public SubmissionsController(IAdminSubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    public async Task<IActionResult> Index()
    {
        var submissions = await _submissionService.GetPendingSubmissionsAsync();
        var vm = new SubmissionsIndexViewModel
        {
            Submissions = submissions
        };
        return View(vm);
    }

    public async Task<IActionResult> Review(int id)
    {
        var submission = await _submissionService.GetSubmissionForReviewAsync(id);
        if (submission is null)
        {
            return NotFound();
        }

        var vm = new SubmissionReviewViewModel
        {
            Submission = submission
        };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id) 
    {
       await _submissionService.ApproveAsync(id);
       return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        await _submissionService.RejectAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
