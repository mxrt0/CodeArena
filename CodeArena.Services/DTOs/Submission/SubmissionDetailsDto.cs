using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Submission;

public sealed record SubmissionDetailsDto(
    int SubmissionId,
    string ChallengeSlug,
    string ChallengeTitle,
    string Language,
    string Status,
    string? Feedback,
    string SolutionCode,
    DateTime SubmittedAt
);


