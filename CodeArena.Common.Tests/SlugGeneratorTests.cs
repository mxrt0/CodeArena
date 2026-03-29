namespace CodeArena.Common.Tests;
using CodeArena.Common.Utilities;
using NUnit.Framework;
using System.Collections.Generic;


[TestFixture]
public class SlugGeneratorTests
{
    [Test]
    public void Generate_WhenSimpleTitle_ReturnsLowercaseHyphenated()
    {
        var result = SlugGenerator.Generate("Test Challenge");

        Assert.That(result, Is.EqualTo("test-challenge"));
    }

    [Test]
    public void Generate_WhenUppercaseAndExtraSpaces_NormalizesCorrectly()
    {
        var result = SlugGenerator.Generate("   TEST    Challenge   ");

        Assert.That(result, Is.EqualTo("test-challenge"));
    }

    [Test]
    public void Generate_WhenSpecialCharacters_RemovesThem()
    {
        var result = SlugGenerator.Generate("C# & .NET!!");

        Assert.That(result, Is.EqualTo("c-net"));
    }

    [Test]
    public void Generate_WhenMultipleSpaces_CollapsesToSingleHyphen()
    {
        var result = SlugGenerator.Generate("a    b     c");

        Assert.That(result, Is.EqualTo("a-b-c"));
    }

    [Test]
    public void Generate_WhenAlreadySlugLike_ReturnsSame()
    {
        var result = SlugGenerator.Generate("already-slug");

        Assert.That(result, Is.EqualTo("already-slug"));
    }

    [Test]
    public void GenerateUnique_WhenNoExistingSlugs_ReturnsBaseSlug()
    {
        var existing = new HashSet<string>();

        var result = SlugGenerator.GenerateUnique("Test Challenge", existing);

        Assert.That(result, Is.EqualTo("test-challenge"));
    }

    [Test]
    public void GenerateUnique_WhenSlugExists_AppendsOne()
    {
        var existing = new HashSet<string> { "test-challenge" };

        var result = SlugGenerator.GenerateUnique("Test Challenge", existing);

        Assert.That(result, Is.EqualTo("test-challenge-1"));
    }

    [Test]
    public void GenerateUnique_WhenMultipleConflicts_AppendsNextAvailableNumber()
    {
        var existing = new HashSet<string>
        {
            "test-challenge",
            "test-challenge-1",
            "test-challenge-2"
        };

        var result = SlugGenerator.GenerateUnique("Test Challenge", existing);

        Assert.That(result, Is.EqualTo("test-challenge-3"));
    }

    [Test]
    public void GenerateUnique_WhenBaseSlugEndsWithNumber_StillAppendsCorrectly()
    {
        var existing = new HashSet<string>
        {
            "test-1",
            "test-1-1"
        };

        var result = SlugGenerator.GenerateUnique("Test 1", existing);

        Assert.That(result, Is.EqualTo("test-1-2"));
    }

    [Test]
    public void GenerateUnique_WhenNoConflictWithDifferentSlug_ReturnsBaseSlug()
    {
        var existing = new HashSet<string> { "another-slug" };

        var result = SlugGenerator.GenerateUnique("Test Challenge", existing);

        Assert.That(result, Is.EqualTo("test-challenge"));
    }
}