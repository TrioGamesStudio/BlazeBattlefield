using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankSystem
{
    public static readonly string[] RankNames =
    {
        "Recruit", "Private", "Corporal", "Sergeant", "Lieutenant", "Captain", "Major", "Colonel", "General", "Legend"
    };

    public static readonly int[] XPThresholds =
    {
        100, 500, 1000, 3000, 5000, 10000, 15000, 20000, 30000, 50000
    };

    public static string GetRankName(int xp)
    {
        for (int i = XPThresholds.Length - 1; i >= 0; i--)
        {
            if (xp >= XPThresholds[i])
                return RankNames[i + 1];
        }
        return RankNames[0]; // Default to "Recruit"
    }

    public static int GetNextThreshold(int currentRank)
    {
        return currentRank < XPThresholds.Length - 1 ? XPThresholds[currentRank] : int.MaxValue;
    }
}
