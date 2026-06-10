using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Iteration04Builder
{
    private const string LevelsFolder = "Assets/SwimGame/Levels";

    private static readonly string Level2Layout = string.Join("\n",
        "########",
        "#P.....#",
        "#.####.#",
        "#..<...#",
        "####.###",
        "#......#",
        "#.#..#.#",
        "#......#",
        "#.##.###",
        "#..r...#",
        "#####.E#",
        "########");

    private static readonly string Level3Layout = string.Join("\n",
        "##########",
        "#P.......#",
        "#.######.#",
        "#...>....#",
        "#######..#",
        "#........#",
        "#.######.#",
        "#..L.....#",
        "####..####",
        "#........#",
        "#.######.#",
        "#....6...#",
        "#######..#",
        "#........#",
        "#.......E#",
        "##########");

    [MenuItem("SwimGame/Update Test Levels (Iteration 4)")]
    public static void UpdateTestLevels()
    {
        UpdateLevel("TestLevel2_Medium", Level2Layout, new List<string> { "1,4 6,4 6,6 1,6" });
        UpdateLevel("TestLevel3_Big", Level3Layout, new List<string>());
        AssetDatabase.SaveAssets();
        Debug.Log("Test levels updated with all AI types (Iteration 4). TestLevel1 untouched.");
    }

    private static void UpdateLevel(string fileName, string layout, List<string> patrolRoutes)
    {
        string path = LevelsFolder + "/" + fileName + ".asset";
        var data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (data == null)
        {
            Debug.LogWarning("Level asset not found: " + path + ". Run Create Test Levels (Iteration 2) first.");
            return;
        }
        data.layout = layout;
        data.patrolRoutes = patrolRoutes;
        EditorUtility.SetDirty(data);
    }
}
