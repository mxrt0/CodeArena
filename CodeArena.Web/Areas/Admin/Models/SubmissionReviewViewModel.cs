using CodeArena.Services.DTOs.Admin.Submission;
using System.ComponentModel.DataAnnotations;

namespace CodeArena.Web.Areas.Admin.Models;

public class SubmissionReviewViewModel
{
    public AdminSubmissionReviewDto Submission { get; set; } = null!;

    [Display(Name = "Your Feedback (optional)")]
    public string? SubmissionFeedback { get; set; }
}
