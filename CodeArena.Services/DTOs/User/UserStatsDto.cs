using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.User;

public sealed record UserStatsDto(
    int SolvedTotal,
    int EasySolved,
    int MediumSolved,
    int HardSolved,
    int PendingSubmissions,
    int RejectedSubmissions,
    int TotalXp,
    int Level,
    int CurrentLevelXp, 
    int NextLevelXp
);
