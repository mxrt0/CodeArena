using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Challenge;

public sealed record ChallengeDisplayDto(
    int Id,
    string Title,
    string Description,
    string Difficulty,
    string[] Tags,
    int SubmissionCount,
    bool IsDeleted
)
{
    public bool IsSolved { get; set; }
}


