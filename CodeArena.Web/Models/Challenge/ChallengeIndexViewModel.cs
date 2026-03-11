using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Models.Challenge;

public class ChallengeIndexViewModel
{
    public IEnumerable<ChallengeDisplayDto> Challenges { get; set; } = new List<ChallengeDisplayDto>();
    public string? SelectedDifficulty { get; set; }
    public ChallengeStatus StatusFilter { get; set; }
}
