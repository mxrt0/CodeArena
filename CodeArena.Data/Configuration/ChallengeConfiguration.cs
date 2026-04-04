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
            Description = "Write a function that takes two numbers and returns their sum. Example: Input: 3, 5 → Output: 8.",
            Slug ="sum-two-numbers"
        },
        new Challenge
        {
            Id = 2,
            Title = "FizzBuzz",
            Difficulty = Difficulty.Medium,
            Tags = "loops",
            Description = "Write a program that prints numbers from 1 to 100. For multiples of 3, print 'Fizz' instead of the number, for multiples of 5 print 'Buzz', and for multiples of both 3 and 5 print 'FizzBuzz'.",
            Slug = "fizzbuzz"
        },
        new Challenge
    {
        Id = 3,
        Title = "Reverse a String",
        Difficulty = Difficulty.Easy,
        Tags = "strings, algorithms",
        Description = "Write a function that reverses a given string. Example: Input: 'hello' → Output: 'olleh'.",
        Slug = "reverse-a-string"
    },
    new Challenge
    {
        Id = 4,
        Title = "Factorial",
        Difficulty = Difficulty.Medium,
        Tags = "recursion, math",
        Description = "Write a function that returns the factorial of a given non-negative integer n.",
        Slug = "factorial"
    },
    new Challenge
    {
        Id = 5,
        Title = "Palindrome Check",
        Difficulty = Difficulty.Easy,
        Tags = "strings, algorithms",
        Description = "Determine if a given string is a palindrome. Example: 'racecar' → true, 'hello' → false.",
        Slug = "palindrome-check"
    },
    new Challenge
    {
        Id = 6,
        Title = "Fibonacci Sequence",
        Difficulty = Difficulty.Medium,
        Tags = "recursion, math, algorithms",
        Description = "Return the first n numbers of the Fibonacci sequence.",
        Slug = "fibonacci-sequence"
    },
    new Challenge
    {
        Id = 7,
        Title = "Find Maximum in Array",
        Difficulty = Difficulty.Easy,
        Tags = "array, algorithms",
        Description = "Write a function that finds the maximum number in a given array.",
        Slug = "find-maximum-in-array"
    },
    new Challenge
    {
        Id = 8,
        Title = "Merge Two Sorted Arrays",
        Difficulty = Difficulty.Medium,
        Tags = "array, sorting, algorithms",
        Description = "Given two sorted arrays, merge them into one sorted array.",
        Slug = "merge-two-sorted-arrays"
    },
    new Challenge
    {
        Id = 9,
        Title = "Count Vowels",
        Difficulty = Difficulty.Easy,
        Tags = "strings",
        Description = "Count the number of vowels in a given string.",
        Slug = "count-vowels"
    },
    new Challenge
    {
        Id = 10,
        Title = "Reverse Words in Sentence",
        Difficulty = Difficulty.Medium,
        Tags = "strings, algorithms",
        Description = "Reverse the order of words in a sentence. Example: 'Hello World' → 'World Hello'.",
        Slug = "reverse-words-in-sentence"
    },
    new Challenge
    {
        Id = 11,
        Title = "Sum of Array Elements",
        Difficulty = Difficulty.Easy,
        Tags = "array, math",
        Description = "Return the sum of all elements in a given integer array.",
        Slug = "sum-of-array-elements"
    },
    new Challenge
    {
        Id = 12,
        Title = "Remove Duplicates from Array",
        Difficulty = Difficulty.Medium,
        Tags = "array, algorithms",
        Description = "Return a new array with all duplicates removed from the original array.",
        Slug = "remove-duplicates-from-array"
    },
    new Challenge
    {
        Id = 13,
        Title = "Prime Number Check",
        Difficulty = Difficulty.Easy,
        Tags = "math, algorithms",
        Description = "Check if a given number is a prime number.",
        Slug = "prime-number-check"
    },
    new Challenge
    {
        Id = 14,
        Title = "Intersection of Two Arrays",
        Difficulty = Difficulty.Medium,
        Tags = "array, algorithms",
        Description = "Return the intersection elements of two arrays.",
        Slug = "intersection-of-two-arrays"
    },
    new Challenge
    {
        Id = 15,
        Title = "Caesar Cipher",
        Difficulty = Difficulty.Medium,
        Tags = "strings, algorithms, encryption",
        Description = "Encrypt a string using a Caesar cipher with a given shift value.",
        Slug = "caesar-cipher"
    },
    new Challenge
    {
        Id = 16,
        Title = "Find Second Largest",
        Difficulty = Difficulty.Medium,
        Tags = "array, algorithms",
        Description = "Return the second largest number from an array of integers.",
        Slug = "find-second-largest"
    },
    new Challenge
    {
        Id = 17,
        Title = "Sum of Digits",
        Difficulty = Difficulty.Easy,
        Tags = "math, recursion",
        Description = "Calculate the sum of digits of a given integer.",
        Slug = "sum-of-digits"
    },
    new Challenge
    {
        Id = 18,
        Title = "Rotate Array",
        Difficulty = Difficulty.Medium,
        Tags = "array, algorithms",
        Description = "Rotate an array to the right by k steps.",
        Slug = "rotate-array"
    },
    new Challenge
    {
        Id = 19,
        Title = "Balanced Brackets",
        Difficulty = Difficulty.Medium,
        Tags = "stack, algorithms, strings",
        Description = "Check if a string of brackets is balanced. Example: '{[()]}'' → true, '{[(])}' → false.",
        Slug = "balanced-brackets"
    },
    new Challenge
    {
        Id = 20,
        Title = "Count Words in String",
        Difficulty = Difficulty.Easy,
        Tags = "strings",
        Description = "Count the number of words in a given sentence.",
        Slug = "count-words-in-string"
    },
    new Challenge
    {
        Id = 21,
        Title = "Merge Intervals",
        Difficulty = Difficulty.Hard,
        Tags = "array, algorithms, sorting",
        Description = "Given a collection of intervals, merge all overlapping intervals.",
        Slug = "merge-intervals"
    },
    new Challenge
    {
        Id = 22,
        Title = "Longest Substring Without Repeating Characters",
        Difficulty = Difficulty.Hard,
        Tags = "strings, algorithms, sliding-window",
        Description = "Find the length of the longest substring without repeating characters.",
        Slug = "longest-substring-without-repeating-characters"
    },
    new Challenge
    {
        Id = 23,
        Title = "Two Sum",
        Difficulty = Difficulty.Medium,
        Tags = "array, algorithms, hashing",
        Description = "Given an array of integers, return indices of the two numbers such that they add up to a specific target.",
        Slug = "two-sum"
    },
    new Challenge
    {
        Id = 24,
        Title = "Find Missing Number",
        Difficulty = Difficulty.Medium,
        Tags = "array, math",
        Description = "Find the missing number in an array containing n distinct numbers taken from 0 to n.",
        Slug = "find-missing-number"
    },
    new Challenge
    {
        Id = 25,
        Title = "Longest Common Prefix",
        Difficulty = Difficulty.Medium,
        Tags = "strings, algorithms",
        Description = "Find the longest common prefix string amongst an array of strings.",
        Slug = "longest-common-prefix"
    },
    new Challenge
    {
        Id = 26,
        Title = "Valid Anagram",
        Difficulty = Difficulty.Medium,
        Tags = "strings, algorithms",
        Description = "Check if two strings are anagrams of each other.",
        Slug = "valid-anagram"
    },
    new Challenge
    {
        Id = 27,
        Title = "Power of Two",
        Difficulty = Difficulty.Easy,
        Tags = "math, bit-manipulation",
        Description = "Check if a number is a power of two.",
        Slug = "power-of-two"
    }
    };

    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.Property(c => c.Difficulty)
               .HasConversion<string>();

        builder.HasData(data);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasIndex(c => c.Slug)
            .IsUnique();
    }
}

