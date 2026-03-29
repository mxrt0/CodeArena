using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CodeArena.Services.Tests;

[TestFixture]
public class AdminDashboardServiceTests
{
    [Test]
    public async Task GetDashboardDataAsync_WhenCalled_ReturnsCorrectCounts()
    {
        var userRepoMock = new Mock<IUserRepository>();
        var submissionRepoMock = new Mock<ISubmissionRepository>();
        var challengeRepoMock = new Mock<IChallengeRepository>();

        userRepoMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>?>()))
            .ReturnsAsync(5);

        challengeRepoMock
            .Setup(r => r.CountAsync(It.IsAny<Expression<Func<Challenge, bool>>?>()))
            .ReturnsAsync(3);

        // IMPORTANT: sequence for ALL submission calls
        submissionRepoMock
            .SetupSequence(r => r.CountAsync(It.IsAny<Expression<Func<Submission, bool>>?>()))
            .ReturnsAsync(10) // TotalSubmissions (null)
            .ReturnsAsync(4)  // Pending
            .ReturnsAsync(3)  // Approved
            .ReturnsAsync(3); // Rejected

        var service = new AdminDashboardService(
            userRepoMock.Object,
            submissionRepoMock.Object,
            challengeRepoMock.Object
        );

        var result = await service.GetDashboardDataAsync();

        Assert.That(result.TotalUsers, Is.EqualTo(5));
        Assert.That(result.TotalChallenges, Is.EqualTo(3));
        Assert.That(result.TotalSubmissions, Is.EqualTo(10));
        Assert.That(result.PendingSubmissions, Is.EqualTo(4));
        Assert.That(result.ApprovedSubmissions, Is.EqualTo(3));
        Assert.That(result.RejectedSubmissions, Is.EqualTo(3));
    }
}