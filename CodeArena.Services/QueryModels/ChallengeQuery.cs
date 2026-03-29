using CodeArena.Common.Enums;
using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.QueryModels;

public class ChallengeQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public ChallengeStatus Status { get; set; } = ChallengeStatus.All;
    public Difficulty? Difficulty { get; set; }

    public List<string>? Tags { get; set; }
    public string? Search { get; set; }
}
