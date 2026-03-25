using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Submission;

public sealed record SubmissionDisplayDto(
    int SubmissionId,
    int ChallengeId,
    string ChallengeSlug,
    string ChallengeTitle,
    string Language,
    string Status,
    string? Feedback,
    DateTime SubmittedAt
);
