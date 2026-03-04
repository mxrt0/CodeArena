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
    [StringLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;

    [Required]
    public Difficulty Difficulty { get; set; }
}
