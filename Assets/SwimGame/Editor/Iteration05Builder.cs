using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Iteration05Builder
{
    private const string LevelsFolder = "Assets/SwimGame/Levels";

    private static readonly string Level4Layout = string.Join("\n",
        "##########",
        "#P.......#",
        "#.######.#",
        "#........#",
        "#.######.#",
        "#........#",
        "#.######.#",
        "#........#",
        "#.######.#",
        "#....>...#",
        "#.######.#",
        "#........#",
        "#.......E#",
        "##########");

    [MenuItem("SwimGame/Create Dynamics Test Level (Iteration 5)")]
    public static void CreateDynamicsTestLevel()
    {
        string path = LevelsFolder + "/TestLevel4_Dynamics.asset";
        var data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        bool isNew = data == null;

        if (isNew)
        {
            data = ScriptableObject.CreateInstance<LevelData>();
        }

        data.levelName = "Test Level 4";
        data.layout = Level4Layout;
        data.patrolRoutes = new List<string>();
        data.currentZones = new List<string> { "1,10 8,10 down" };
        data.boats = new List<string> { "2,8 3 right" };
        data.platforms = new List<string> { "3,6 2 2 0", "6,6 2 2 2" };

        if (isNew)
        {
            AssetDatabase.CreateAsset(data, path);
        }
        else
        {
            EditorUtility.SetDirty(data);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Dynamics test level created/updated: " + path);
    }
}
