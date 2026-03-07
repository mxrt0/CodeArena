using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeArena.Web.Controllers;

[Authorize]
public class BaseController : Controller
{
}
