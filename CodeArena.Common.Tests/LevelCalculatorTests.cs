using CodeArena.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Common.Tests;

[TestFixture]
public class LevelCalculatorTests
{
    [Test]
    public void CalculateLevel_WithZeroXp_ReturnsLevelOne()
    {
        var level = LevelCalculator.CalculateLevel(0);

        Assert.That(level, Is.EqualTo(1));
    }

    [Test]
    public void CalculateLevel_WithIncreasingXp_IncreasesLevel()
    {
        var lowXpLevel = LevelCalculator.CalculateLevel(100);
        var highXpLevel = LevelCalculator.CalculateLevel(1000);

        Assert.That(highXpLevel, Is.GreaterThan(lowXpLevel));
    }

    [Test]
    public void GetXpForLevel_LevelOne_ReturnsZero()
    {
        var xp = LevelCalculator.GetXpForLevel(1);

        Assert.That(xp, Is.EqualTo(0));
    }

    [Test]
    public void GetXpForLevel_HigherLevel_RequiresMoreXp()
    {
        var xpLevel2 = LevelCalculator.GetXpForLevel(2);
        var xpLevel5 = LevelCalculator.GetXpForLevel(5);

        Assert.That(xpLevel5, Is.GreaterThan(xpLevel2));
    }

    [Test]
    public void GetXpBounds_ReturnsValidRange()
    {
        var (current, next) = LevelCalculator.GetXpBounds(3);

        Assert.That(next, Is.GreaterThan(current));
    }

    [Test]
    public void GetProgress_WithinBounds_IsCorrect()
    {
        var totalXp = 500;

        var (currentXp, neededXp) = LevelCalculator.GetProgress(totalXp);

        Assert.That(currentXp, Is.GreaterThanOrEqualTo(0));
        Assert.That(currentXp, Is.LessThanOrEqualTo(neededXp));
    }

    [Test]
    public void GetProgress_ExactLevelBoundary_CurrentXpIsZero()
    {
        var level = 4;
        var xpAtLevelStart = LevelCalculator.GetXpForLevel(level);

        var (currentXp, _) = LevelCalculator.GetProgress(xpAtLevelStart);

        Assert.That(currentXp, Is.EqualTo(0));
    }
}
