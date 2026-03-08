using CodeArena.Data.Common.Enums;
using CodeArena.Services.DTOs.Challenge;

namespace CodeArena.Web.Models.Challenge;

public class ChallengeDetailsViewModel
{
    public ChallengeDisplayDto Challenge { get; set; } = null!;

    public string? SolutionCode { get; set; }
    public SubmissionLanguage? Language { get; set; }

    public bool HasPendingSubmission { get; set; }  
    public bool HasApprovedSubmission { get; set; } 
}
