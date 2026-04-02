using CodeArena.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Configuration;

public class XpTransactionConfiguration : IEntityTypeConfiguration<XpTransaction>
{
    public void Configure(EntityTypeBuilder<XpTransaction> builder)
    {
        builder.Property(x => x.Reason)
               .HasConversion<string>();

        builder.HasIndex(x => new { x.UserId, x.ChallengeId, x.Reason })
                .IsUnique();
    }
}

