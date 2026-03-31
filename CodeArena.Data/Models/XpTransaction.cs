using CodeArena.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Models;

public class XpTransaction
{
    public int Id { get; set; }
    public int XpAmount { get; set; }
    public XpReason Reason { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    [ForeignKey(nameof(Challenge))]
    public int? ChallengeId { get; set; }
    public Challenge? Challenge { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

