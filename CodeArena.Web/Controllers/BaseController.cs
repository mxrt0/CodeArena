using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeArena.Web.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class BaseController : Controller
{
    protected string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
