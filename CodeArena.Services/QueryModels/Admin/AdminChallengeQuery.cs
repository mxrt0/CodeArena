using CodeArena.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.QueryModels.Admin;

public class AdminChallengeQuery : ChallengeQuery
{
    public ChallengeState? State { get; set; } = ChallengeState.All;
}
