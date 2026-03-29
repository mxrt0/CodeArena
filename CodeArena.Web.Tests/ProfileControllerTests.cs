using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeArena.Web.Controllers;
using CodeArena.Services.Core.Contracts;
using CodeArena.Web.Models.User;
using CodeArena.Services.DTOs.User;

namespace CodeArena.Web.Tests;

[TestFixture]
public class ProfileControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private ProfileController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();

        _controller = new ProfileController(_userServiceMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                }, "mock"))
            }
        };
    }
    [TearDown]
    public void ControllerCleanup()
    {
        _controller.Dispose();
    }

    [Test]
    public async Task Stats_ReturnsViewWithUserStatsViewModel()
    {
        var dummyStats = new UserStatsDto(10, 5, 3, 2, 1, 2);

        _userServiceMock
            .Setup(s => s.GetUserStatsAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(dummyStats);

        var result = await _controller.Stats() as ViewResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Model, Is.InstanceOf<UserStatsViewModel>());

        var model = result.Model as UserStatsViewModel;
        Assert.That(model!.Stats, Is.EqualTo(dummyStats));

        _userServiceMock.Verify(s => s.GetUserStatsAsync(_controller.User), Times.Once);
    }
}