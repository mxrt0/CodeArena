using CodeArena.Data.Models;
using CodeArena.Data.Repositories.Contracts;
using CodeArena.Services.Core.Contracts;
using CodeArena.Services.DTOs.User;
using Microsoft.AspNetCore.Identity;
using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeArena.Services.Core;

public class UserService : IUserService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        ISubmissionRepository repository,
        UserManager<ApplicationUser> userManager
    )
    {
        _submissionRepository = repository;
        _userManager = userManager;
    }

    public async Task<UserStatsDto> GetUserStatsAsync(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);

        var submissions = _submissionRepository.GetAll()
            .Where(s => s.UserId == userId);

        var solved = await submissions
            .Where(s => s.Status == SubmissionStatus.Approved)
            .Include(s => s.Challenge)
            .ToListAsync();

        return new UserStatsDto(
            solved.Count,
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Easy),
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Medium),
            solved.Count(s => s.Challenge.Difficulty == Difficulty.Hard),
            await submissions.CountAsync(s => s.Status == SubmissionStatus.Pending),
            await submissions.CountAsync(s => s.Status == SubmissionStatus.Rejected)
        );
    }
}
