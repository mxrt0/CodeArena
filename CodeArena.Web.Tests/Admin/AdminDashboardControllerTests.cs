using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Dashboard;
using CodeArena.Web.Areas.Admin.Controllers;
using CodeArena.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CodeArena.Web.Tests.Admin;

[TestFixture]
public class DashboardControllerTests
{
    private Mock<IAdminDashboardService> _dashboardServiceMock = null!;
    private DashboardController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _dashboardServiceMock = new Mock<IAdminDashboardService>();
        _controller = new DashboardController(_dashboardServiceMock.Object);
    }
    [TearDown]
    public void ControllerCleanup()
    {
        _controller.Dispose();
    }
    [Test]
    public async Task Index_ReturnsViewWithDashboardData()
    {
        var dto = new AdminDashboardDto(1, 1, 1, 1, 1, 1);
        _dashboardServiceMock
            .Setup(s => s.GetDashboardDataAsync())
            .ReturnsAsync(dto);

        var result = await _controller.Index() as ViewResult;

        Assert.That(result, Is.Not.Null);

        var model = result!.Model as AdminDashboardViewModel;
        Assert.That(model, Is.Not.Null);
        Assert.That(model!.DashboardData, Is.EqualTo(dto));

        Assert.That(_controller.ViewData["ActivePage"], Is.EqualTo("Dashboard"));

        _dashboardServiceMock.Verify(s => s.GetDashboardDataAsync(), Times.Once);
    }
}