using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.Challenge;

namespace CodeArena.Data.Models;

public class Challenge
{
    public int Id { get; set; }

    [MaxLength(TitleMaxLength)]
    public string Title { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
    public string Description { get; set; } = null!;

    [MaxLength(TagsMaxLength)]
    public string Tags { get; set; } = null!;

    public Difficulty Difficulty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public string? Slug { get; set; }
    public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
}

