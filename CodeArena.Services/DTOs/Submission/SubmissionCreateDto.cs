using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Submission;

namespace CodeArena.Services.DTOs.Submission;

public class SubmissionCreateDto
{
    [Required]
    public int ChallengeId { get; set; }

    [Required(ErrorMessage = "Solution is required.")]
    [StringLength(SolutionCodeMaxLength, MinimumLength = 5, ErrorMessage = "Submission must be between 5 and 10000 characters.")]
    public string SolutionCode { get; set; } = null!;

    [Required(ErrorMessage = "Please select a language.")]
    public SubmissionLanguage Language { get; set; }
}

