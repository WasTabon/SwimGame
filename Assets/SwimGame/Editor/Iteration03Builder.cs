using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration03Builder
{
    private const string LevelsFolder = "Assets/SwimGame/Levels";
    private static readonly Color WindowColor = new Color32(22, 33, 62, 255);

    private static readonly string Level1Layout = string.Join("\n",
        "######",
        "#P...#",
        "#.##.#",
        "#..>.#",
        "#.##.#",
        "#....#",
        "#...E#",
        "######");

    private static readonly string Level2Layout = string.Join("\n",
        "########",
        "#P.....#",
        "#.####.#",
        "#..<...#",
        "####.###",
        "#......#",
        "#.####.#",
        "#...v..#",
        "#.##.###",
        "#......#",
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
        "#..<.....#",
        "####..####",
        "#........#",
        "#.######.#",
        "#....>...#",
        "#######..#",
        "#........#",
        "#.......E#",
        "##########");

    [MenuItem("SwimGame/Update Test Levels (Iteration 3)")]
    public static void UpdateTestLevels()
    {
        UpdateLevel("TestLevel1_Small", Level1Layout);
        UpdateLevel("TestLevel2_Medium", Level2Layout);
        UpdateLevel("TestLevel3_Big", Level3Layout);
        AssetDatabase.SaveAssets();
        Debug.Log("Test levels updated with NPCs (Iteration 3)");
    }

    private static void UpdateLevel(string fileName, string layout)
    {
        string path = LevelsFolder + "/" + fileName + ".asset";
        var data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (data == null)
        {
            Debug.LogWarning("Level asset not found: " + path + ". Run Create Test Levels (Iteration 2) first.");
            return;
        }
        data.layout = layout;
        EditorUtility.SetDirty(data);
    }

    [MenuItem("SwimGame/Update Game Scene (Iteration 3)")]
    public static void UpdateGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var gameRoot = GameObject.Find("GameRoot");
        if (gameRoot == null)
        {
            Debug.LogError("GameRoot not found. Run Build Game Scene (Iteration 2) first.");
            return;
        }

        var gridManager = gameRoot.GetComponent<GridManager>();
        var loader = gameRoot.GetComponent<LevelLoader>();
        var highlighter = gameRoot.GetComponent<MoveHighlighter>();
        var playerController = Object.FindObjectOfType<PlayerController>();
        var cameraController = Object.FindObjectOfType<CameraController>();
        var hud = Object.FindObjectOfType<GameHUD>();
        var canvas = hud.transform;
        var safeArea = canvas.Find("SafeArea");

        var turnManager = gameRoot.GetComponent<TurnManager>();
        if (turnManager == null) turnManager = gameRoot.AddComponent<TurnManager>();

        var swimmersContainer = gameRoot.transform.Find("SwimmersContainer");
        if (swimmersContainer == null)
        {
            var go = new GameObject("SwimmersContainer");
            go.transform.SetParent(gameRoot.transform, false);
            swimmersContainer = go.transform;
        }

        var waitTr = safeArea.Find("WaitButton");
        SwimGameEditorUtils.SetRect(waitTr, new Vector2(0.5f, 0f), new Vector2(-235f, 170f), new Vector2(440f, 150f));

        Button restartButton;
        var restartTr = safeArea.Find("RestartButton");
        if (restartTr != null)
        {
            restartButton = restartTr.GetComponent<Button>();
        }
        else
        {
            restartButton = SwimGameEditorUtils.CreateButton(safeArea, "RestartButton", "RESTART",
                SwimGameEditorUtils.PrimaryColor, 56f);
            SwimGameEditorUtils.SetRect(restartButton.transform, new Vector2(0.5f, 0f), new Vector2(235f, 170f), new Vector2(440f, 150f));
        }

        var winPopup = BuildWinPopup(canvas, loader);
        var losePopup = BuildLosePopup(canvas, loader);

        var tmSo = new SerializedObject(turnManager);
        tmSo.FindProperty("gridManager").objectReferenceValue = gridManager;
        tmSo.FindProperty("player").objectReferenceValue = playerController;
        tmSo.FindProperty("highlighter").objectReferenceValue = highlighter;
        tmSo.FindProperty("cameraController").objectReferenceValue = cameraController;
        tmSo.FindProperty("hud").objectReferenceValue = hud;
        tmSo.ApplyModifiedPropertiesWithoutUndo();

        var loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("turnManager").objectReferenceValue = turnManager;
        loaderSo.FindProperty("winPopup").objectReferenceValue = winPopup;
        loaderSo.FindProperty("losePopup").objectReferenceValue = losePopup;
        loaderSo.FindProperty("swimmersContainer").objectReferenceValue = swimmersContainer;
        loaderSo.ApplyModifiedPropertiesWithoutUndo();

        var hudSo = new SerializedObject(hud);
        hudSo.FindProperty("restartButton").objectReferenceValue = restartButton;
        hudSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated (Iteration 3)");
    }

    private static WinPopup BuildWinPopup(Transform canvas, LevelLoader loader)
    {
        var existing = canvas.Find("WinPopup");
        if (existing != null) return existing.GetComponent<WinPopup>();

        var shell = BuildPopupShell(canvas, "WinPopup", new Vector2(820f, 900f));

        var title = SwimGameEditorUtils.CreateText(shell.window, "Title", "LEVEL COMPLETE!", 76f, Color.white);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 320f), new Vector2(760f, 140f));

        var movesValue = SwimGameEditorUtils.CreateText(shell.window, "MovesValue", "Moves: 0", 56f,
            SwimGameEditorUtils.AccentColor);
        SwimGameEditorUtils.SetRect(movesValue, new Vector2(0.5f, 0.5f), new Vector2(0f, 140f), new Vector2(600f, 110f));

        var restartBtn = SwimGameEditorUtils.CreateButton(shell.window, "RestartButton", "RESTART",
            SwimGameEditorUtils.AccentColor, 60f);
        SwimGameEditorUtils.SetRect(restartBtn.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(560f, 150f));

        var menuBtn = SwimGameEditorUtils.CreateButton(shell.window, "MenuButton", "MENU",
            SwimGameEditorUtils.PrimaryColor, 52f);
        SwimGameEditorUtils.SetRect(menuBtn.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -320f), new Vector2(560f, 130f));

        var popup = shell.root.AddComponent<WinPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = shell.backdrop;
        so.FindProperty("window").objectReferenceValue = shell.window;
        so.FindProperty("movesValueText").objectReferenceValue = movesValue.GetComponent<TextMeshProUGUI>();
        so.FindProperty("restartButton").objectReferenceValue = restartBtn;
        so.FindProperty("menuButton").objectReferenceValue = menuBtn;
        so.FindProperty("levelLoader").objectReferenceValue = loader;
        so.ApplyModifiedPropertiesWithoutUndo();

        shell.root.SetActive(false);
        return popup;
    }

    private static LosePopup BuildLosePopup(Transform canvas, LevelLoader loader)
    {
        var existing = canvas.Find("LosePopup");
        if (existing != null) return existing.GetComponent<LosePopup>();

        var shell = BuildPopupShell(canvas, "LosePopup", new Vector2(820f, 800f));

        var title = SwimGameEditorUtils.CreateText(shell.window, "Title", "OUCH!", 90f, new Color32(231, 76, 60, 255));
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 270f), new Vector2(700f, 150f));

        var subtitle = SwimGameEditorUtils.CreateText(shell.window, "Subtitle", "You bumped into a swimmer", 44f, Color.white);
        SwimGameEditorUtils.SetRect(subtitle, new Vector2(0.5f, 0.5f), new Vector2(0f, 120f), new Vector2(720f, 100f));

        var restartBtn = SwimGameEditorUtils.CreateButton(shell.window, "RestartButton", "RESTART",
            SwimGameEditorUtils.AccentColor, 60f);
        SwimGameEditorUtils.SetRect(restartBtn.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -80f), new Vector2(560f, 150f));

        var menuBtn = SwimGameEditorUtils.CreateButton(shell.window, "MenuButton", "MENU",
            SwimGameEditorUtils.PrimaryColor, 52f);
        SwimGameEditorUtils.SetRect(menuBtn.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -270f), new Vector2(560f, 130f));

        var popup = shell.root.AddComponent<LosePopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = shell.backdrop;
        so.FindProperty("window").objectReferenceValue = shell.window;
        so.FindProperty("restartButton").objectReferenceValue = restartBtn;
        so.FindProperty("menuButton").objectReferenceValue = menuBtn;
        so.FindProperty("levelLoader").objectReferenceValue = loader;
        so.ApplyModifiedPropertiesWithoutUndo();

        shell.root.SetActive(false);
        return popup;
    }

    private static (GameObject root, Image backdrop, RectTransform window) BuildPopupShell(
        Transform canvas, string name, Vector2 windowSize)
    {
        var rootGo = new GameObject(name, typeof(RectTransform));
        rootGo.transform.SetParent(canvas, false);
        var rootRt = rootGo.GetComponent<RectTransform>();
        rootRt.anchorMin = Vector2.zero;
        rootRt.anchorMax = Vector2.one;
        rootRt.offsetMin = Vector2.zero;
        rootRt.offsetMax = Vector2.zero;

        var backdropGo = new GameObject("Backdrop", typeof(RectTransform));
        backdropGo.transform.SetParent(rootGo.transform, false);
        var backdrop = backdropGo.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.6f);
        var backdropRt = backdrop.rectTransform;
        backdropRt.anchorMin = Vector2.zero;
        backdropRt.anchorMax = Vector2.one;
        backdropRt.offsetMin = Vector2.zero;
        backdropRt.offsetMax = Vector2.zero;

        var window = SwimGameEditorUtils.CreatePanel(rootGo.transform, "Window", WindowColor);
        SwimGameEditorUtils.SetRect(window, new Vector2(0.5f, 0.5f), Vector2.zero, windowSize);

        return (rootGo, backdrop, window);
    }
}
