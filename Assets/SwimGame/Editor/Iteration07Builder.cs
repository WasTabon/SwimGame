using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration07Builder
{
    private class LevelDef
    {
        public string fileName;
        public string levelName;
        public int optimalMoves;
        public string layout;
        public string[] patrols;
        public string[] currents;
        public string[] boats;
        public string[] platforms;
    }

    private const string CampaignFolder = "Assets/SwimGame/Levels/Campaign";
    private const string DatabasePath = "Assets/SwimGame/Levels/LevelDatabase.asset";

    private static LevelDef[] Defs()
    {
        return new[]
        {
            new LevelDef
            {
                fileName = "Level_01",
                levelName = "Level 1",
                optimalMoves = 7,
                layout = string.Join("\n",
                "######",
                "#P...#",
                "#....#",
                "#....#",
                "#....#",
                "#...E#",
                "######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_02",
                levelName = "Level 2",
                optimalMoves = 14,
                layout = string.Join("\n",
                "######",
                "#P...#",
                "####.#",
                "#....#",
                "#.####",
                "#....#",
                "#...E#",
                "######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_03",
                levelName = "Level 3",
                optimalMoves = 10,
                layout = string.Join("\n",
                "######",
                "#P...#",
                "####.#",
                "#>...#",
                "###.##",
                "#....#",
                "#...E#",
                "######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_04",
                levelName = "Level 4",
                optimalMoves = 23,
                layout = string.Join("\n",
                "#######",
                "#P....#",
                "#####.#",
                "#.>...#",
                "###.###",
                "#.....#",
                "#####.#",
                "#...<.#",
                "###.###",
                "#....E#",
                "#######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_05",
                levelName = "Level 5",
                optimalMoves = 20,
                layout = string.Join("\n",
                "######",
                "#P...#",
                "###.##",
                "###.##",
                "###.##",
                "##..##",
                "###^##",
                "###.##",
                "###E##",
                "######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_06",
                levelName = "Level 6",
                optimalMoves = 8,
                layout = string.Join("\n",
                "#######",
                "#P....#",
                "#.....#",
                "#.....#",
                "#.....#",
                "#....E#",
                "#######"),
                patrols = new[] { "2,2 4,2 4,4 2,4" },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_07",
                levelName = "Level 7",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#......#",
                "#......#",
                "#####.##",
                "#.>....#",
                "####.###",
                "#.....E#",
                "########"),
                patrols = new[] { "2,5 5,5 5,7 2,7" },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_08",
                levelName = "Level 8",
                optimalMoves = 22,
                layout = string.Join("\n",
                "#######",
                "#P....#",
                "#####.#",
                "#..r..#",
                "###.###",
                "#..l..#",
                "#####.#",
                "#....E#",
                "#######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_09",
                levelName = "Level 9",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#..L...#",
                "#......#",
                "#####.##",
                "#...R..#",
                "####.###",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_10",
                levelName = "Level 10",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#...L..#",
                "#......#",
                "#####.##",
                "#.>....#",
                "####.###",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_11",
                levelName = "Level 11",
                optimalMoves = 11,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#......#",
                "#...6..#",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_12",
                levelName = "Level 12",
                optimalMoves = 16,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#..6...#",
                "#......#",
                "####.###",
                "#......#",
                "#...4..#",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_13",
                levelName = "Level 13",
                optimalMoves = 6,
                layout = string.Join("\n",
                "#######",
                "#P....#",
                "#.....#",
                "#.....#",
                "#.....#",
                "#.....#",
                "#....E#",
                "#######"),
                patrols = new string[] {  },
                currents = new[] { "3,1 3,6 down" },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_14",
                levelName = "Level 14",
                optimalMoves = 9,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#......#",
                "#......#",
                "#...<..#",
                "#......#",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new[] { "1,5 6,5 down" },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_15",
                levelName = "Level 15",
                optimalMoves = 16,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#.<....#",
                "####.###",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new[] { "1,7 6,7 right" },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_16",
                levelName = "Level 16",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#......#",
                "####.###",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new[] { "1,5 2 right" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_17",
                levelName = "Level 17",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#......#",
                "######.#",
                "#......#",
                "####.###",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new[] { "1,5 6,5 right" },
                boats = new[] { "1,3 2 left" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_18",
                levelName = "Level 18",
                optimalMoves = 8,
                layout = string.Join("\n",
                "#######",
                "#P....#",
                "#.###.#",
                "#.....#",
                "#.###.#",
                "#....E#",
                "#######"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "1,4 2 2 2", "5,4 2 2 0" }
            },
            new LevelDef
            {
                fileName = "Level_19",
                levelName = "Level 19",
                optimalMoves = 34,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "######.#",
                "#......#",
                "#.######",
                "#......#",
                "######.#",
                "#......#",
                "#.######",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "6,8 3 1 0", "1,6 3 1 2", "6,4 3 1 0", "1,2 3 1 2" }
            },
            new LevelDef
            {
                fileName = "Level_20",
                levelName = "Level 20",
                optimalMoves = 14,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#.>....#",
                "####.###",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "5,6 2 2 0", "5,2 2 2 2" }
            },
            new LevelDef
            {
                fileName = "Level_21",
                levelName = "Level 21",
                optimalMoves = 18,
                layout = string.Join("\n",
                "#########",
                "#P......#",
                "#.......#",
                "#.......#",
                "#.......#",
                "######.##",
                "#.......#",
                "####.####",
                "#......E#",
                "#########"),
                patrols = new[] { "2,5 6,5 6,7 2,7" },
                currents = new string[] {  },
                boats = new[] { "1,3 3 right" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_22",
                levelName = "Level 22",
                optimalMoves = 16,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#..R...#",
                "####.###",
                "#......#",
                "#####.##",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new[] { "1,3 6,3 right" },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_23",
                levelName = "Level 23",
                optimalMoves = 22,
                layout = string.Join("\n",
                "########",
                "#P.....#",
                "#####.##",
                "#..r...#",
                "####.###",
                "#......#",
                "#####.##",
                "#..l...#",
                "###.####",
                "#.....E#",
                "########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "4,5 2 2 0", "3,1 2 2 2" }
            },
            new LevelDef
            {
                fileName = "Level_24",
                levelName = "Level 24",
                optimalMoves = 18,
                layout = string.Join("\n",
                "#########",
                "#P......#",
                "#.......#",
                "#...6...#",
                "#.......#",
                "######.##",
                "#.......#",
                "####.####",
                "#......E#",
                "#########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new[] { "1,3 3 left" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_25",
                levelName = "Level 25",
                optimalMoves = 17,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#........#",
                "#........#",
                "#........#",
                "#######.##",
                "#.<......#",
                "#####.####",
                "#........#",
                "#.......E#",
                "##########"),
                patrols = new[] { "2,6 7,6 7,8 2,8" },
                currents = new[] { "1,2 8,2 right" },
                boats = new string[] {  },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_26",
                levelName = "Level 26",
                optimalMoves = 32,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#######.##",
                "#..L.....#",
                "#####.####",
                "#........#",
                "#######.##",
                "#...r....#",
                "####.#####",
                "#.......E#",
                "##########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "7,8 2 2 0", "4,2 2 2 2" }
            },
            new LevelDef
            {
                fileName = "Level_27",
                levelName = "Level 27",
                optimalMoves = 18,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#........#",
                "#...6....#",
                "#........#",
                "#######.##",
                "#........#",
                "####.#####",
                "#.......E#",
                "##########"),
                patrols = new string[] {  },
                currents = new[] { "1,3 8,3 left" },
                boats = new[] { "1,3 3 right" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_28",
                levelName = "Level 28",
                optimalMoves = 19,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#........#",
                "#........#",
                "#........#",
                "#######.##",
                "#...>....#",
                "#####.####",
                "#.......E#",
                "##########"),
                patrols = new[] { "2,5 7,5 7,7 2,7" },
                currents = new string[] {  },
                boats = new string[] {  },
                platforms = new[] { "7,4 2 2 0", "5,2 2 2 2" }
            },
            new LevelDef
            {
                fileName = "Level_29",
                levelName = "Level 29",
                optimalMoves = 25,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#..R.....#",
                "#........#",
                "#....6...#",
                "#........#",
                "#######.##",
                "#........#",
                "####.#####",
                "#.......E#",
                "##########"),
                patrols = new string[] {  },
                currents = new string[] {  },
                boats = new[] { "1,1 3 right" },
                platforms = new string[] {  }
            },
            new LevelDef
            {
                fileName = "Level_30",
                levelName = "Level 30",
                optimalMoves = 28,
                layout = string.Join("\n",
                "##########",
                "#P.......#",
                "#........#",
                "#........#",
                "#######.##",
                "#..>.....#",
                "#####.####",
                "#........#",
                "#######.##",
                "#........#",
                "####.#####",
                "#.......E#",
                "##########"),
                patrols = new[] { "2,9 7,9 7,10 2,10" },
                currents = new[] { "1,5 8,5 right" },
                boats = new[] { "1,3 3 right" },
                platforms = new[] { "7,8 2 2 0", "4,2 2 2 2" }
            }
        };
    }

    [MenuItem("SwimGame/Create 30 Levels + Database (Iteration 7)")]
    public static void CreateLevelsAndDatabase()
    {
        SwimGameEditorUtils.EnsureFolder("Assets/SwimGame/Levels");
        SwimGameEditorUtils.EnsureFolder(CampaignFolder);

        var defs = Defs();
        var levels = new List<LevelData>();

        foreach (var def in defs)
        {
            string path = CampaignFolder + "/" + def.fileName + ".asset";
            var asset = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<LevelData>();
                AssetDatabase.CreateAsset(asset, path);
            }
            asset.levelName = def.levelName;
            asset.optimalMoves = def.optimalMoves;
            asset.layout = def.layout;
            asset.patrolRoutes = new List<string>(def.patrols);
            asset.currentZones = new List<string>(def.currents);
            asset.boats = new List<string>(def.boats);
            asset.platforms = new List<string>(def.platforms);
            EditorUtility.SetDirty(asset);
            levels.Add(asset);
        }

        var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<LevelDatabase>();
            AssetDatabase.CreateAsset(database, DatabasePath);
        }
        database.levels = levels;
        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Created/updated 30 campaign levels and LevelDatabase");
    }

    [MenuItem("SwimGame/Build Level Select Scene (Iteration 7)")]
    public static void BuildLevelSelectScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DatabasePath);
        if (database == null)
        {
            Debug.LogError("LevelDatabase not found. Run 'Create 30 Levels + Database (Iteration 7)' first.");
            return;
        }

        var scene = SwimGameEditorUtils.CreateEmptyScene();
        SwimGameEditorUtils.CreateCamera();
        SwimGameEditorUtils.CreateEventSystem();
        SwimGameEditorUtils.CreateBootstrap();

        var canvasGo = SwimGameEditorUtils.CreateCanvas();
        var safeArea = SwimGameEditorUtils.CreateSafeArea(canvasGo.transform);

        var bg = SwimGameEditorUtils.CreatePanel(canvasGo.transform, "Background", SwimGameEditorUtils.BackgroundColor);
        bg.SetAsFirstSibling();
        bg.anchorMin = Vector2.zero;
        bg.anchorMax = Vector2.one;
        bg.offsetMin = Vector2.zero;
        bg.offsetMax = Vector2.zero;

        var title = SwimGameEditorUtils.CreateText(safeArea, "Title", "LEVELS", 96f, Color.white);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 1f), new Vector2(0f, -140f), new Vector2(700f, 120f));

        var backButton = SwimGameEditorUtils.CreateButton(safeArea, "BackButton", "<", SwimGameEditorUtils.PrimaryColor, 56f);
        SwimGameEditorUtils.SetRect(backButton.transform, new Vector2(0f, 1f), new Vector2(110f, -140f), new Vector2(120f, 120f));

        var gridGo = new GameObject("LevelGrid", typeof(RectTransform));
        gridGo.transform.SetParent(safeArea, false);
        var gridRt = gridGo.GetComponent<RectTransform>();
        gridRt.anchorMin = new Vector2(0.5f, 0.5f);
        gridRt.anchorMax = new Vector2(0.5f, 0.5f);
        gridRt.pivot = new Vector2(0.5f, 0.5f);
        gridRt.anchoredPosition = new Vector2(0f, -80f);
        gridRt.sizeDelta = new Vector2(950f, 1230f);
        var grid = gridGo.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(170f, 190f);
        grid.spacing = new Vector2(16f, 16f);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        grid.childAlignment = TextAnchor.UpperCenter;

        var ui = canvasGo.AddComponent<LevelSelectUI>();
        var uiSo = new SerializedObject(ui);
        uiSo.FindProperty("database").objectReferenceValue = database;
        uiSo.FindProperty("backButton").objectReferenceValue = backButton;
        uiSo.FindProperty("gridContainer").objectReferenceValue = gridRt;
        uiSo.ApplyModifiedPropertiesWithoutUndo();

        SwimGameEditorUtils.SaveScene(scene, "LevelSelect");
        Debug.Log("LevelSelect scene built");
    }

    [MenuItem("SwimGame/Update Game Scene (Iteration 7)")]
    public static void UpdateGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        var database = AssetDatabase.LoadAssetAtPath<LevelDatabase>(DatabasePath);
        if (database == null)
        {
            Debug.LogError("LevelDatabase not found. Run 'Create 30 Levels + Database (Iteration 7)' first.");
            return;
        }

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var gameRoot = GameObject.Find("GameRoot");
        if (gameRoot == null)
        {
            Debug.LogError("GameRoot not found. Run previous iteration builders first.");
            return;
        }

        var loader = gameRoot.GetComponent<LevelLoader>();
        var hud = Object.FindObjectOfType<GameHUD>();
        var canvas = hud.GetComponent<Canvas>() != null ? hud.transform : hud.transform.root;

        var oldWinPopup = Object.FindObjectOfType<WinPopup>(true);
        Transform popupParent = canvas;
        int siblingIndex = -1;
        if (oldWinPopup != null)
        {
            popupParent = oldWinPopup.transform.parent;
            siblingIndex = oldWinPopup.transform.GetSiblingIndex();
            Object.DestroyImmediate(oldWinPopup.gameObject);
        }

        var winPopup = BuildWinPopup(popupParent, loader);
        if (siblingIndex >= 0) winPopup.transform.SetSiblingIndex(siblingIndex);

        var loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("database").objectReferenceValue = database;
        loaderSo.FindProperty("winPopup").objectReferenceValue = winPopup;
        loaderSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated for campaign levels (Iteration 7)");
    }

    private static WinPopup BuildWinPopup(Transform parent, LevelLoader loader)
    {
        var rootGo = new GameObject("WinPopup", typeof(RectTransform));
        rootGo.transform.SetParent(parent, false);
        var rootRt = rootGo.GetComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var backdropGo = new GameObject("Backdrop", typeof(RectTransform));
        backdropGo.transform.SetParent(rootGo.transform, false);
        var backdropRt = backdropGo.GetComponent<RectTransform>();
        backdropRt.anchorMin = Vector2.zero;
        backdropRt.anchorMax = Vector2.one;
        backdropRt.offsetMin = Vector2.zero;
        backdropRt.offsetMax = Vector2.zero;
        var backdrop = backdropGo.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.6f);

        var window = SwimGameEditorUtils.CreatePanel(rootGo.transform, "Window", SwimGameEditorUtils.PanelColor);
        SwimGameEditorUtils.SetRect(window, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(850f, 1050f));

        var title = SwimGameEditorUtils.CreateText(window, "Title", "LEVEL COMPLETE!", 72f, SwimGameEditorUtils.AccentColor);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 400f), new Vector2(800f, 100f));

        var starImages = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            var starGo = new GameObject("Star_" + i, typeof(RectTransform));
            starGo.transform.SetParent(window, false);
            SwimGameEditorUtils.SetRect(starGo.transform, new Vector2(0.5f, 0.5f), new Vector2((i - 1) * 150f, 220f), new Vector2(110f, 110f));
            starImages[i] = starGo.AddComponent<Image>();
            starImages[i].color = new Color32(44, 62, 80, 255);
            starImages[i].raycastTarget = false;
        }

        var movesValue = SwimGameEditorUtils.CreateText(window, "MovesValue", "Moves: 0    Best: 0", 52f, Color.white);
        SwimGameEditorUtils.SetRect(movesValue, new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(800f, 80f));

        var nextButton = SwimGameEditorUtils.CreateButton(window, "NextButton", "NEXT", SwimGameEditorUtils.AccentColor, 56f);
        SwimGameEditorUtils.SetRect(nextButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -140f), new Vector2(520f, 130f));

        var restartButton = SwimGameEditorUtils.CreateButton(window, "RestartButton", "RESTART", SwimGameEditorUtils.PrimaryColor, 56f);
        SwimGameEditorUtils.SetRect(restartButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -320f), new Vector2(520f, 130f));

        var menuButton = SwimGameEditorUtils.CreateButton(window, "MenuButton", "MENU", SwimGameEditorUtils.PrimaryColor, 48f);
        SwimGameEditorUtils.SetRect(menuButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -470f), new Vector2(420f, 110f));

        var popup = rootGo.AddComponent<WinPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = backdrop;
        so.FindProperty("window").objectReferenceValue = window;
        so.FindProperty("movesValueText").objectReferenceValue = movesValue.GetComponent<TextMeshProUGUI>();
        so.FindProperty("nextButton").objectReferenceValue = nextButton;
        so.FindProperty("restartButton").objectReferenceValue = restartButton;
        so.FindProperty("menuButton").objectReferenceValue = menuButton;
        so.FindProperty("levelLoader").objectReferenceValue = loader;
        var starsProp = so.FindProperty("starImages");
        starsProp.arraySize = 3;
        for (int i = 0; i < 3; i++)
        {
            starsProp.GetArrayElementAtIndex(i).objectReferenceValue = starImages[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);
        return popup;
    }
}