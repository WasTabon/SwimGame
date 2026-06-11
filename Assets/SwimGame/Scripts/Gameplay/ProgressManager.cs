using UnityEngine;

public static class ProgressManager
{
    private const string StarsKeyPrefix = "level_stars_";

    public static int GetStars(int levelIndex)
    {
        return PlayerPrefs.GetInt(StarsKeyPrefix + levelIndex, 0);
    }

    public static void SetStars(int levelIndex, int stars)
    {
        if (stars <= GetStars(levelIndex)) return;
        PlayerPrefs.SetInt(StarsKeyPrefix + levelIndex, stars);
        PlayerPrefs.Save();
    }

    public static bool IsUnlocked(int levelIndex)
    {
        return levelIndex == 0 || GetStars(levelIndex - 1) > 0;
    }

    public static int CalculateStars(int moves, int optimalMoves)
    {
        if (moves <= optimalMoves) return 3;
        if (moves <= optimalMoves + 3) return 2;
        return 1;
    }
}
