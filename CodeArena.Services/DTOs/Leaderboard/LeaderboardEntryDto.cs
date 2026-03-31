using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Leaderboard;

public class LeaderboardEntryDto
{
    public string UserId { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int TotalXp { get; set; }
    public int ChallengesSolved { get; set; }
    public int Rank { get; set; }
}
