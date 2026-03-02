using CodeArena.Services.Core;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Submission;
using CodeArena.Web.Models.Challenge;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Controllers
{
    public class SubmissionsController : BaseController
    {
        private readonly ISubmissionService _submissionService;
        private readonly IChallengeService _challengeService;

        public SubmissionsController(
            ISubmissionService submissionService,
            IChallengeService challengeService)
        {
            _submissionService = submissionService;
            _challengeService = challengeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(SubmissionCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                var challenge = await _challengeService.GetChallengeByIdAsync(createDto.ChallengeId);
                if (challenge is null)
                {
                    return NotFound();
                }

                var vm = new ChallengeDetailsViewModel
                {
                    Challenge = challenge,
                    SolutionCode = createDto.SolutionCode,
                    Language = createDto.Language
                };

                return View("~/Views/Challenges/Details.cshtml", vm);
            }
            await _submissionService.CreateSubmissionAsync(createDto, User);
            return RedirectToAction("Details", "Challenges", new {id = createDto.ChallengeId});
        }
    }
}
