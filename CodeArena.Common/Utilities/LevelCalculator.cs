using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeArena.Common.Utilities;

public static class LevelCalculator
{
    private const double Multiplier = 50;
    public static int CalculateLevel(int totalXp)
    {
        return (int)Math.Floor(1 + Math.Pow(totalXp / Multiplier, 0.6));
    }
    public static int GetXpForLevel(int level)
    {
        if (level <= 1) return 0;

        return (int)(Multiplier * Math.Pow(level - 1, 1 / 0.6));
    }
    public static (int CurrentLevelXp, int NextLevelXp) GetXpBounds(int level)
    {
        var current = GetXpForLevel(level);
        var next = GetXpForLevel(level + 1);

        return (current, next);
    }
    public static (int CurrentXp, int NeededXp) GetProgress(int totalXp)
    {
        var level = CalculateLevel(totalXp);
        var (currentLevelXp, nextLevelXp) = GetXpBounds(level);

        var currentXp = totalXp - currentLevelXp;
        var neededXp = nextLevelXp - currentLevelXp;

        return (currentXp, neededXp);
    }
}
