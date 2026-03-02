using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models;
using CodeArena.Web.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeArena.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IChallengeService _challengeService;
        public HomeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        public async Task<IActionResult> Index()
        {
            var challenges = await _challengeService.GetChallengesAsync();
            var vm = new HomeIndexViewModel
            {
            };
            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
