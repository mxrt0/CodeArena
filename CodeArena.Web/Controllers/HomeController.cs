using CodeArena.Data.Models;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models;
using CodeArena.Web.Models.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CodeArena.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IChallengeService _challengeService;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(
            IChallengeService challengeService,
            UserManager<ApplicationUser> userManager)
        {
            _challengeService = challengeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is not null && await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return LocalRedirect("/admin");
                }
            }
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

        [Route("Home/Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == StatusCodes.Status400BadRequest)
            { 
                return View("BadRequest");
            }
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                return View("ServerError");
            }
            if (statusCode == StatusCodes.Status404NotFound)
            {
                return View("NotFound");
            }
            if (statusCode == StatusCodes.Status403Forbidden)
            {
                return View("Forbidden");
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
