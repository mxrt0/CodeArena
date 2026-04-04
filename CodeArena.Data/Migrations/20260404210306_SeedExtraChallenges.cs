using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeArena.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedExtraChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8754), "Write a function that takes two numbers and returns their sum.\n Example: Input: 3, 5 → Output: 8." });

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8760), "Write a program that prints numbers from 1 to 100.\n For multiples of 3, print 'Fizz' instead of the number, for multiples of 5 print 'Buzz', and for multiples of both 3 and 5 print 'FizzBuzz'." });

            migrationBuilder.InsertData(
                table: "Challenges",
                columns: new[] { "Id", "CreatedAt", "Description", "Difficulty", "IsDeleted", "Slug", "Tags", "Title" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8764), "Determine if a given string is a palindrome.\n Example: 'racecar' → true, 'hello' → false.", "Easy", false, "palindrome-check", "strings, algorithms", "Palindrome Check" },
                    { 6, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8765), "Return the first n numbers of the Fibonacci sequence.", "Medium", false, "fibonacci-sequence", "recursion, math, algorithms", "Fibonacci Sequence" },
                    { 7, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8766), "Write a function that finds the maximum number in a given array.", "Easy", false, "find-maximum-in-array", "array, algorithms", "Find Maximum in Array" },
                    { 8, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8767), "Given two sorted arrays, merge them into one sorted array.", "Medium", false, "merge-two-sorted-arrays", "array, sorting, algorithms", "Merge Two Sorted Arrays" },
                    { 9, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8768), "Count the number of vowels in a given string.", "Easy", false, "count-vowels", "strings", "Count Vowels" },
                    { 10, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8769), "Reverse the order of words in a sentence. Example: 'Hello World' → 'World Hello'.", "Medium", false, "reverse-words-in-sentence", "strings, algorithms", "Reverse Words in Sentence" },
                    { 11, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8770), "Return the sum of all elements in a given integer array.", "Easy", false, "sum-of-array-elements", "array, math", "Sum of Array Elements" },
                    { 12, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8771), "Return a new array with all duplicates removed from the original array.", "Medium", false, "remove-duplicates-from-array", "array, algorithms", "Remove Duplicates from Array" },
                    { 13, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8773), "Check if a given number is a prime number.", "Easy", false, "prime-number-check", "math, algorithms", "Prime Number Check" },
                    { 14, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8774), "Return the intersection elements of two arrays.", "Medium", false, "intersection-of-two-arrays", "array, algorithms", "Intersection of Two Arrays" },
                    { 15, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8775), "Encrypt a string using a Caesar cipher with a given shift value.", "Medium", false, "caesar-cipher", "strings, algorithms, encryption", "Caesar Cipher" },
                    { 16, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8776), "Return the second largest number from an array of integers.", "Medium", false, "find-second-largest", "array, algorithms", "Find Second Largest" },
                    { 17, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8777), "Calculate the sum of digits of a given integer.", "Easy", false, "sum-of-digits", "math, recursion", "Sum of Digits" },
                    { 18, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8778), "Rotate an array to the right by k steps.", "Medium", false, "rotate-array", "array, algorithms", "Rotate Array" },
                    { 19, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8779), "Check if a string of brackets is balanced.\n Example: '{[()]}'' → true, '{[(])}' → false.", "Medium", false, "balanced-brackets", "stack, algorithms, strings", "Balanced Brackets" },
                    { 20, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8780), "Count the number of words in a given sentence.", "Easy", false, "count-words-in-string", "strings", "Count Words in String" },
                    { 21, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8781), "Given a collection of intervals, merge all overlapping intervals.", "Hard", false, "merge-intervals", "array, algorithms, sorting", "Merge Intervals" },
                    { 22, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8782), "Find the length of the longest substring without repeating characters.", "Hard", false, "longest-substring-without-repeating-characters", "strings, algorithms, sliding-window", "Longest Substring Without Repeating Characters" },
                    { 24, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8783), "Find the missing number in an array containing n distinct numbers taken from 0 to n.", "Medium", false, "find-missing-number", "array, math", "Find Missing Number" },
                    { 25, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8785), "Find the longest common prefix string amongst an array of strings.", "Medium", false, "longest-common-prefix", "strings, algorithms", "Longest Common Prefix" },
                    { 26, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8786), "Check if two strings are anagrams of each other.", "Medium", false, "valid-anagram", "strings, algorithms", "Valid Anagram" },
                    { 27, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8815), "Check if a number is a power of two.", "Easy", false, "power-of-two", "math, bit-manipulation", "Power of Two" },
                    { 67, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8761), "Write a function that reverses a given string.\n Example: Input: 'hello' → Output: 'olleh'.", "Easy", false, "reverse-a-string", "strings, algorithms", "Reverse a String" },
                    { 69, new DateTime(2026, 4, 4, 21, 3, 6, 185, DateTimeKind.Utc).AddTicks(8763), "Write a function that returns the factorial of a given non-negative integer n.", "Medium", false, "factorial", "recursion, math", "Factorial" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 3, 31, 15, 37, 43, 487, DateTimeKind.Utc).AddTicks(6277), "Write a function that takes two numbers and returns their sum. Example: Input: 3, 5 → Output: 8." });

            migrationBuilder.UpdateData(
                table: "Challenges",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 3, 31, 15, 37, 43, 487, DateTimeKind.Utc).AddTicks(6281), "Write a program that prints numbers from 1 to 100. For multiples of 3, print 'Fizz' instead of the number, for multiples of 5 print 'Buzz', and for multiples of both 3 and 5 print 'FizzBuzz'." });
        }
    }
}
