using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration02Builder
{
    private const string LevelsFolder = "Assets/SwimGame/Levels";

    private static readonly string Level1Layout = string.Join("\n",
        "######",
        "#P...#",
        "#.##.#",
        "#....#",
        "#.##.#",
        "#....#",
        "#...E#",
        "######");

    private static readonly string Level2Layout = string.Join("\n",
        "########",
        "#P.....#",
        "#.####.#",
        "#......#",
        "####.###",
        "#......#",
        "#.####.#",
        "#......#",
        "#.##.###",
        "#......#",
        "#####.E#",
        "########");

    private static readonly string Level3Layout = string.Join("\n",
        "##########",
        "#P.......#",
        "#.######.#",
        "#........#",
        "#######..#",
        "#........#",
        "#.######.#",
        "#........#",
        "####..####",
        "#........#",
        "#.######.#",
        "#........#",
        "#######..#",
        "#........#",
        "#.......E#",
        "##########");

    [MenuItem("SwimGame/Create Test Levels (Iteration 2)")]
    public static void CreateTestLevels()
    {
        EnsureTestLevels();
        AssetDatabase.SaveAssets();
        Debug.Log("Test levels created in " + LevelsFolder);
    }

    [MenuItem("SwimGame/Build Game Scene (Iteration 2)")]
    public static void BuildGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        LevelData firstLevel = EnsureTestLevels();
        AssetDatabase.SaveAssets();

        var scene = SwimGameEditorUtils.CreateEmptyScene();

        var cameraGo = SwimGameEditorUtils.CreateCamera();
        var cameraController = cameraGo.AddComponent<CameraController>();
        var camera = cameraGo.GetComponent<Camera>();

        SwimGameEditorUtils.CreateEventSystem();

        var canvasGo = SwimGameEditorUtils.CreateCanvas();
        var safeArea = SwimGameEditorUtils.CreateSafeArea(canvasGo.transform);

        var topBar = SwimGameEditorUtils.CreatePanel(safeArea, "TopBar", SwimGameEditorUtils.PanelColor);
        SwimGameEditorUtils.StretchHorizontally(topBar, 1f, 160f, 0f, new Vector2(0.5f, 1f));

        var backButton = SwimGameEditorUtils.CreateButton(topBar, "BackButton", "BACK",
            SwimGameEditorUtils.PrimaryColor, 44f);
        SwimGameEditorUtils.SetRect(backButton.transform, new Vector2(0f, 0.5f), new Vector2(135f, 0f), new Vector2(220f, 110f));

        var levelNameRect = SwimGameEditorUtils.CreateText(topBar, "LevelNameText", "Level", 56f, Color.white);
        SwimGameEditorUtils.SetRect(levelNameRect, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(480f, 110f));

        var movesRect = SwimGameEditorUtils.CreateText(topBar, "MovesText", "Moves: 0", 44f, Color.white);
        SwimGameEditorUtils.SetRect(movesRect, new Vector2(1f, 0.5f), new Vector2(-185f, 0f), new Vector2(330f, 110f));

        var waitButton = SwimGameEditorUtils.CreateButton(safeArea, "WaitButton", "WAIT",
            SwimGameEditorUtils.AccentColor, 60f);
        SwimGameEditorUtils.SetRect(waitButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 170f), new Vector2(460f, 150f));

        var root = new GameObject("GameRoot");
        var gridManager = root.AddComponent<GridManager>();
        var inputController = root.AddComponent<InputController>();
        var highlighter = root.AddComponent<MoveHighlighter>();
        var loader = root.AddComponent<LevelLoader>();

        var cellsContainer = new GameObject("CellsContainer").transform;
        cellsContainer.SetParent(root.transform, false);

        var playerGo = new GameObject("Player");
        playerGo.transform.SetParent(root.transform, false);
        var playerController = playerGo.AddComponent<PlayerController>();
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(playerGo.transform, false);
        var visualRenderer = visualGo.AddComponent<SpriteRenderer>();

        var hud = canvasGo.AddComponent<GameHUD>();

        Wire(gridManager, "cellsContainer", cellsContainer);
        Wire(playerController, "visualRenderer", visualRenderer);

        var inputSo = new SerializedObject(inputController);
        inputSo.FindProperty("gameCamera").objectReferenceValue = camera;
        inputSo.FindProperty("gridManager").objectReferenceValue = gridManager;
        inputSo.FindProperty("cameraController").objectReferenceValue = cameraController;
        inputSo.ApplyModifiedPropertiesWithoutUndo();

        var loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("defaultLevel").objectReferenceValue = firstLevel;
        loaderSo.FindProperty("gridManager").objectReferenceValue = gridManager;
        loaderSo.FindProperty("player").objectReferenceValue = playerController;
        loaderSo.FindProperty("highlighter").objectReferenceValue = highlighter;
        loaderSo.FindProperty("cameraController").objectReferenceValue = cameraController;
        loaderSo.FindProperty("inputController").objectReferenceValue = inputController;
        loaderSo.FindProperty("hud").objectReferenceValue = hud;
        loaderSo.ApplyModifiedPropertiesWithoutUndo();

        var hudSo = new SerializedObject(hud);
        hudSo.FindProperty("levelNameText").objectReferenceValue = levelNameRect.GetComponent<TMPro.TextMeshProUGUI>();
        hudSo.FindProperty("movesText").objectReferenceValue = movesRect.GetComponent<TMPro.TextMeshProUGUI>();
        hudSo.FindProperty("backButton").objectReferenceValue = backButton;
        hudSo.FindProperty("waitButton").objectReferenceValue = waitButton;
        hudSo.FindProperty("levelLoader").objectReferenceValue = loader;
        hudSo.ApplyModifiedPropertiesWithoutUndo();

        SwimGameEditorUtils.CreateBootstrap();
        SwimGameEditorUtils.SaveScene(scene, "Game");
        Debug.Log("Game scene built (Iteration 2)");
    }

    private static void Wire(Component target, string property, Object value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(property).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static LevelData EnsureTestLevels()
    {
        SwimGameEditorUtils.EnsureFolder(LevelsFolder);
        LevelData first = CreateLevelAsset("TestLevel1_Small", "Test Level 1", Level1Layout);
        CreateLevelAsset("TestLevel2_Medium", "Test Level 2", Level2Layout);
        CreateLevelAsset("TestLevel3_Big", "Test Level 3", Level3Layout);
        return first;
    }

    private static LevelData CreateLevelAsset(string fileName, string levelName, string layout)
    {
        string path = LevelsFolder + "/" + fileName + ".asset";
        var existing = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (existing != null) return existing;

        var data = ScriptableObject.CreateInstance<LevelData>();
        data.levelName = levelName;
        data.layout = layout;
        AssetDatabase.CreateAsset(data, path);
        return data;
    }
}
