using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Challenge;

namespace CodeArena.Services.DTOs.Admin.Challenge;

public class CreateChallengeDto
{
    [Required]
    [StringLength(TitleMaxLength, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters long. ")]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(DescriptionMaxLength, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters long.")]
    public string Description { get; set; } = null!;

    [Required]
    public Difficulty Difficulty { get; set; }

    [Display(Name = "Tags (optional)")]
    public string? Tags { get; set; }
}
