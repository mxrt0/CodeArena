using CodeArena.Services.DTOs.Leaderboard;

namespace CodeArena.Web.Models.Leaderboard;

public class LeaderboardIndexViewModel
{
    public IEnumerable<LeaderboardEntryDto> Leaderboard {  get; set; } 
        = new List<LeaderboardEntryDto>();
    public string CurrentUserId { get; set; } = null!;  
}
