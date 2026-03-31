using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;

namespace CodeArena.Web.Areas.Admin.Models;

public class ChallengesIndexViewModel
{
    public IEnumerable<ChallengeDisplayDto> Challenges { get; set; } 
        = new List<ChallengeDisplayDto>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string? Search { get; set; }
    public ChallengeState? State { get; set; } = ChallengeState.All;
}
