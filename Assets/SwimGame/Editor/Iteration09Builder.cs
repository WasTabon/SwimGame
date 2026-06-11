using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration09Builder
{
    [MenuItem("SwimGame/Update Main Menu Scene (Iteration 9)")]
    public static void UpdateMainMenuScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/MainMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var menuUi = Object.FindObjectOfType<MainMenuUI>();
        var canvas = Object.FindObjectOfType<Canvas>();

        var settingsPopup = Object.FindObjectOfType<SettingsPopup>(true);
        if (settingsPopup == null)
        {
            settingsPopup = BuildSettingsPopup(canvas.transform);
        }

        var so = new SerializedObject(menuUi);
        so.FindProperty("settingsPopup").objectReferenceValue = settingsPopup;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Main menu updated with settings popup (Iteration 9)");
    }

    [MenuItem("SwimGame/Update Game Scene (Iteration 9)")]
    public static void UpdateGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var gameRoot = GameObject.Find("GameRoot");
        var hud = Object.FindObjectOfType<GameHUD>();
        var loader = gameRoot.GetComponent<LevelLoader>();
        var gridManager = gameRoot.GetComponent<GridManager>();
        var turnManager = gameRoot.GetComponent<TurnManager>();
        var playerController = Object.FindObjectOfType<PlayerController>();
        var safeArea = hud.transform.Find("SafeArea");
        var topBar = safeArea.Find("TopBar");

        SwimGameEditorUtils.SetRect(topBar.Find("MovesText"), new Vector2(1f, 0.5f), new Vector2(-300f, 0f), new Vector2(330f, 110f));

        var pauseTr = topBar.Find("PauseButton");
        Button pauseButton;
        if (pauseTr != null)
        {
            pauseButton = pauseTr.GetComponent<Button>();
        }
        else
        {
            pauseButton = SwimGameEditorUtils.CreateButton(topBar, "PauseButton", "II", SwimGameEditorUtils.PrimaryColor, 44f);
            SwimGameEditorUtils.SetRect(pauseButton.transform, new Vector2(1f, 0.5f), new Vector2(-70f, 0f), new Vector2(110f, 110f));
        }

        var popupParent = Object.FindObjectOfType<WinPopup>(true).transform.parent;

        var settingsPopup = Object.FindObjectOfType<SettingsPopup>(true);
        if (settingsPopup == null)
        {
            settingsPopup = BuildSettingsPopup(popupParent);
        }

        var pausePopup = Object.FindObjectOfType<PausePopup>(true);
        if (pausePopup == null)
        {
            pausePopup = BuildPausePopup(popupParent, settingsPopup, loader);
        }

        var tutorial = gameRoot.GetComponent<TutorialController>();
        if (tutorial == null) tutorial = gameRoot.AddComponent<TutorialController>();

        var tutorialSo = new SerializedObject(tutorial);
        tutorialSo.FindProperty("gridManager").objectReferenceValue = gridManager;
        tutorialSo.FindProperty("player").objectReferenceValue = playerController;
        tutorialSo.FindProperty("turnManager").objectReferenceValue = turnManager;
        tutorialSo.ApplyModifiedPropertiesWithoutUndo();

        var hudSo = new SerializedObject(hud);
        hudSo.FindProperty("pauseButton").objectReferenceValue = pauseButton;
        hudSo.FindProperty("pausePopup").objectReferenceValue = pausePopup;
        hudSo.ApplyModifiedPropertiesWithoutUndo();

        var loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("tutorialController").objectReferenceValue = tutorial;
        loaderSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with pause, settings and tutorial (Iteration 9)");
    }

    private static RectTransform BuildPopupShell(Transform parent, string name, Vector2 windowSize,
        out Image backdrop, out GameObject rootGo)
    {
        rootGo = new GameObject(name, typeof(RectTransform));
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
        backdrop = backdropGo.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.6f);

        var window = SwimGameEditorUtils.CreatePanel(rootGo.transform, "Window", SwimGameEditorUtils.PanelColor);
        SwimGameEditorUtils.SetRect(window, new Vector2(0.5f, 0.5f), Vector2.zero, windowSize);
        return window;
    }

    private static SettingsPopup BuildSettingsPopup(Transform parent)
    {
        var window = BuildPopupShell(parent, "SettingsPopup", new Vector2(850f, 920f), out var backdrop, out var rootGo);

        var title = SwimGameEditorUtils.CreateText(window, "Title", "SETTINGS", 76f, SwimGameEditorUtils.AccentColor);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 350f), new Vector2(700f, 100f));

        var soundToggle = CreateSettingsRow(window, "SOUND", 170f);
        var musicToggle = CreateSettingsRow(window, "MUSIC", 20f);
        var vibrationToggle = CreateSettingsRow(window, "VIBRATION", -130f);

        var closeButton = SwimGameEditorUtils.CreateButton(window, "CloseButton", "CLOSE", SwimGameEditorUtils.PrimaryColor, 48f);
        SwimGameEditorUtils.SetRect(closeButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -340f), new Vector2(420f, 110f));

        var popup = rootGo.AddComponent<SettingsPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = backdrop;
        so.FindProperty("window").objectReferenceValue = window;
        so.FindProperty("soundButton").objectReferenceValue = soundToggle;
        so.FindProperty("musicButton").objectReferenceValue = musicToggle;
        so.FindProperty("vibrationButton").objectReferenceValue = vibrationToggle;
        so.FindProperty("closeButton").objectReferenceValue = closeButton;
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);
        return popup;
    }

    private static Button CreateSettingsRow(RectTransform window, string itemName, float y)
    {
        var rowGo = new GameObject("Row_" + itemName, typeof(RectTransform));
        rowGo.transform.SetParent(window, false);
        SwimGameEditorUtils.SetRect(rowGo.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(760f, 120f));

        var nameRect = SwimGameEditorUtils.CreateText(rowGo.transform, "Name", itemName, 48f, Color.white);
        SwimGameEditorUtils.SetRect(nameRect, new Vector2(0f, 0.5f), new Vector2(220f, 0f), new Vector2(440f, 70f));
        nameRect.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

        var toggle = SwimGameEditorUtils.CreateButton(rowGo.transform, "ToggleButton", "ON", SwimGameEditorUtils.AccentColor, 44f);
        SwimGameEditorUtils.SetRect(toggle.transform, new Vector2(1f, 0.5f), new Vector2(-130f, 0f), new Vector2(220f, 100f));
        return toggle;
    }

    private static PausePopup BuildPausePopup(Transform parent, SettingsPopup settingsPopup, LevelLoader loader)
    {
        var window = BuildPopupShell(parent, "PausePopup", new Vector2(750f, 980f), out var backdrop, out var rootGo);

        var title = SwimGameEditorUtils.CreateText(window, "Title", "PAUSE", 80f, SwimGameEditorUtils.AccentColor);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 370f), new Vector2(600f, 100f));

        var resumeButton = SwimGameEditorUtils.CreateButton(window, "ResumeButton", "RESUME", SwimGameEditorUtils.AccentColor, 54f);
        SwimGameEditorUtils.SetRect(resumeButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(520f, 130f));

        var restartButton = SwimGameEditorUtils.CreateButton(window, "RestartButton", "RESTART", SwimGameEditorUtils.PrimaryColor, 54f);
        SwimGameEditorUtils.SetRect(restartButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(520f, 130f));

        var settingsButton = SwimGameEditorUtils.CreateButton(window, "SettingsButton", "SETTINGS", SwimGameEditorUtils.PrimaryColor, 54f);
        SwimGameEditorUtils.SetRect(settingsButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -160f), new Vector2(520f, 130f));

        var menuButton = SwimGameEditorUtils.CreateButton(window, "MenuButton", "MENU", SwimGameEditorUtils.PrimaryColor, 48f);
        SwimGameEditorUtils.SetRect(menuButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -340f), new Vector2(420f, 110f));

        var popup = rootGo.AddComponent<PausePopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = backdrop;
        so.FindProperty("window").objectReferenceValue = window;
        so.FindProperty("resumeButton").objectReferenceValue = resumeButton;
        so.FindProperty("restartButton").objectReferenceValue = restartButton;
        so.FindProperty("settingsButton").objectReferenceValue = settingsButton;
        so.FindProperty("menuButton").objectReferenceValue = menuButton;
        so.FindProperty("settingsPopup").objectReferenceValue = settingsPopup;
        so.FindProperty("levelLoader").objectReferenceValue = loader;
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);
        return popup;
    }
}
