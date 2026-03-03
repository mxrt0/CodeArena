using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Admin.Submission;

public sealed record AdminSubmissionReviewDto(
    int SubmissionId,
    string ChallengeTitle,
    string UserDisplayName,
    string Language,
    string Difficulty,
    string SolutionCode,
    string SubmittedAt
);

