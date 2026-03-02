using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Submission;

namespace CodeArena.Data.Models;

public class Submission
{
    public int Id { get; set; }

    [MaxLength(SolutionCodeMaxLength)]
    public string SolutionCode { get; set; } = null!;

    public SubmissionLanguage Language { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;

    [MaxLength(FeedbackMaxLength)]
    public string? Feedback { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(Challenge))]
    public int ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;
}

