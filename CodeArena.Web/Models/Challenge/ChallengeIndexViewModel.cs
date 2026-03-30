using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;
using Microsoft.AspNetCore.Mvc;

namespace CodeArena.Web.Models.Challenge;

public class ChallengeIndexViewModel
{
    public IEnumerable<ChallengeDisplayDto> Challenges { get; set; } = new List<ChallengeDisplayDto>();
    public Difficulty? SelectedDifficulty { get; set; } = null;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public IEnumerable<string> AvailableTags { get; set; } = new List<string>();
    public ChallengeStatus StatusFilter { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
}
