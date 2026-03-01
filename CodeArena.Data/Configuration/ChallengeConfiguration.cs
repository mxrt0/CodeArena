using CodeArena.Data.Common.Enums;
using CodeArena.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Data.Configuration;

public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
{
    private readonly Challenge[] data =
    {
        new Challenge
        {
            Id = 1,
            Title = "Sum Two Numbers",
            Difficulty = Difficulty.Easy,
            Tags = "math",
            Description = "Write a function that takes two numbers and returns their sum. Example: Input: 3, 5 → Output: 8."
        },
        new Challenge
        {
            Id = 2,
            Title = "FizzBuzz",
            Difficulty = Difficulty.Medium,
            Tags = "loops",
            Description = "Write a program that prints numbers from 1 to 100. For multiples of 3, print 'Fizz' instead of the number, for multiples of 5 print 'Buzz', and for multiples of both 3 and 5 print 'FizzBuzz'."
        }
    };

    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.Property(c => c.Difficulty)
               .HasConversion<string>();

        builder.HasData(data);
    }
}

