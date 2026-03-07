using CodeArena.Services.DTOs.Challenge;

namespace CodeArena.Web.Areas.Admin.Models;

public class ChallengesIndexViewModel
{
    public IEnumerable<ChallengeDisplayDto> Challenges { get; set; } 
        = new List<ChallengeDisplayDto>(); 
}
