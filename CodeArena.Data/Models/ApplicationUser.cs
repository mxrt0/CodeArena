using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArena.Data.Common.EntityValidation.ApplicationUser;

namespace CodeArena.Data.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(DisplayNameMaxLength)]
    public string DisplayName { get; set; } = null!;
    [MaxLength(DisplayNameMaxLength)]
    public string NormalizedDisplayName { get; set; } = null!;

    public int XP { get; set; } = 0;
    public int Level { get; set; } = 1;

    public int CurrentStreak { get; set; } = 0;
    public DateTime? LastSubmissionDate { get; set; } = null;

    public ICollection<Submission> Submissions { get; set; } = new HashSet<Submission>();
}

