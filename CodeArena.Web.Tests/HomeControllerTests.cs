using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.Challenge;
using CodeArena.Web.Controllers;
using CodeArena.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CodeArena.Web.Tests;

[TestFixture]
public class HomeControllerTests
{
    private Mock<IChallengeService> _challengeServiceMock = null!;
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private HomeController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _challengeServiceMock = new Mock<IChallengeService>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var passwordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
        var normalizer = new Mock<ILookupNormalizer>();
        var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();
        var errorDescriber = new Mock<IdentityErrorDescriber>();
        var serviceProvider = new Mock<IServiceProvider>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object,
                options.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                normalizer.Object,
                errorDescriber.Object,
                serviceProvider.Object,
                logger.Object
            );

        _controller = new HomeController(_challengeServiceMock.Object, _userManagerMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };
    }
    [TearDown]
    public void ControllerCleanup()
    {
        _controller.Dispose();
    }
    [Test]
    public async Task Index_AuthenticatedAdmin_RedirectsToAdmin()
    {
        var adminUser = new ApplicationUser { UserName = "admin" };

        _userManagerMock
            .Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(adminUser);
        _userManagerMock
            .Setup(u => u.IsInRoleAsync(adminUser, "Admin"))
            .ReturnsAsync(true);

        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "admin") }, "mock")
        );

        var result = await _controller.Index() as LocalRedirectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Url, Is.EqualTo("/admin"));
    }

    [Test]
    public async Task Index_UnauthenticatedOrNonAdmin_ReturnsView()
    {
        _challengeServiceMock
            .Setup(s => s.GetChallengesAsync(It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<ChallengeStatus>(), It.IsAny<Difficulty>(), It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((Enumerable.Empty<ChallengeDisplayDto>(), 0));

        var result = await _controller.Index() as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<CodeArena.Web.Models.Home.HomeIndexViewModel>());
    }

    [TestCase(StatusCodes.Status400BadRequest, "BadRequest")]
    [TestCase(StatusCodes.Status403Forbidden, "Forbidden")]
    [TestCase(StatusCodes.Status404NotFound, "NotFound")]
    [TestCase(StatusCodes.Status500InternalServerError, "ServerError")]
    public void Error_StatusCode_ReturnsCorrectView(int statusCode, string expectedView)
    {
        var result = _controller.Error(statusCode) as ViewResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ViewName, Is.EqualTo(expectedView));
    }

    [Test]
    public void Error_NoStatusCode_ReturnsDefaultErrorViewModel()
    {
        var result = _controller.Error(null) as ViewResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<ErrorViewModel>());
    }
}