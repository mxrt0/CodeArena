using CodeArena.Data.Common.Enums;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Admin.Contracts;
using CodeArena.Services.DTOs.Admin.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.Core.Admin;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IChallengeRepository _challengeRepository;

    public AdminDashboardService(
        IUserRepository userRepository,
        ISubmissionRepository submissionRepository,
        IChallengeRepository challengeRepository)
    {
        _userRepository = userRepository;
        _submissionRepository = submissionRepository;
        _challengeRepository = challengeRepository;
    }

    public async Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        return new AdminDashboardDto
        (
            TotalUsers: await _userRepository.CountAsync(),
            TotalChallenges: await _challengeRepository.CountAsync(),

            TotalSubmissions: await _submissionRepository.CountAsync(),
            PendingSubmissions:await _submissionRepository
            .CountAsync(s => s.Status == SubmissionStatus.Pending),

            ApprovedSubmissions: await _submissionRepository
            .CountAsync(s => s.Status == SubmissionStatus.Approved),

            RejectedSubmissions: await _submissionRepository
            .CountAsync(s => s.Status == SubmissionStatus.Rejected)
        );
    }
}
