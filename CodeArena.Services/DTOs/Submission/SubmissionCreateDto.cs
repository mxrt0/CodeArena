using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Submission;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Services.DTOs.Submission;

public class SubmissionCreateDto
{
    [Required]
    public int ChallengeId { get; set; }

    [Required(ErrorMessage = SolutionCodeRequiredMessage)]
    [StringLength(SolutionCodeMaxLength, MinimumLength = 5, ErrorMessage = InvalidSolutionCodeLengthMessage)]
    public string SolutionCode { get; set; } = null!;

    [Required(ErrorMessage = LanguageNotSelectedMessage)]
    public SubmissionLanguage Language { get; set; }
}

