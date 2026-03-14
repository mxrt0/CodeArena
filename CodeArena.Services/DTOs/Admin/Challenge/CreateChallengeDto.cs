using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Challenge;
using static CodeArena.Common.OutputMessages;

namespace CodeArena.Services.DTOs.Admin.Challenge;

public class CreateChallengeDto
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = 5, ErrorMessage = InvalidChallengeTitleLengthMessage)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(DescriptionMaxLength, MinimumLength = 10, ErrorMessage = InvalidChallengeDescriptionLengthMessage)]
    public string Description { get; set; } = null!;

    [Required]
    public Difficulty Difficulty { get; set; }

    [Display(Name = "Tags (optional)")]
    public string? Tags { get; set; }
}
