using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Services.DTOs.Admin.Submission;

public sealed record SubmissionDisplayDto(
    int SubmissionId,
    string UserDisplayName,
    string ChallengeTitle,
    string Language,
    DateTime SubmittedAt
);

