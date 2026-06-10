using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Iteration01Builder
{
    private const string ScenesFolder = "Assets/SwimGame/Scenes";
    private static readonly Color BackgroundColor = new Color32(26, 26, 46, 255);
    private static readonly Color PrimaryColor = new Color32(74, 144, 226, 255);
    private static readonly Color AccentColor = new Color32(245, 166, 35, 255);

    [MenuItem("SwimGame/Build Main Menu Scene (Iteration 1)")]
    public static void BuildMainMenuScene()
    {
        if (!CheckTmp()) return;
        var scene = CreateEmptyScene();
        CreateCamera();
        CreateEventSystem();
        var canvasGo = CreateCanvas();
        var safeArea = CreateSafeArea(canvasGo.transform);

        var title = CreateText(safeArea, "Title", "SWIM GAME", 120, Color.white);
        SetRect(title, new Vector2(0.5f, 1f), new Vector2(0f, -400f), new Vector2(1000f, 220f));

        var playButton = CreateButton(safeArea, "PlayButton", "PLAY", AccentColor, 80);
        SetRect(playButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -50f), new Vector2(620f, 190f));

        var settingsButton = CreateButton(safeArea, "SettingsButton", "SETTINGS", PrimaryColor, 56);
        SetRect(settingsButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -320f), new Vector2(460f, 140f));

        var shopButton = CreateButton(safeArea, "ShopButton", "SHOP", PrimaryColor, 56);
        SetRect(shopButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -500f), new Vector2(460f, 140f));

        var menuUI = canvasGo.AddComponent<MainMenuUI>();
        var so = new SerializedObject(menuUI);
        so.FindProperty("playButton").objectReferenceValue = playButton;
        so.FindProperty("settingsButton").objectReferenceValue = settingsButton;
        so.FindProperty("shopButton").objectReferenceValue = shopButton;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateBootstrap();
        SaveScene(scene, "MainMenu");
        Debug.Log("MainMenu scene built (Iteration 1)");
    }

    [MenuItem("SwimGame/Build Game Scene (Iteration 1)")]
    public static void BuildGameScene()
    {
        if (!CheckTmp()) return;
        var scene = CreateEmptyScene();
        CreateCamera();
        CreateEventSystem();
        var canvasGo = CreateCanvas();
        var safeArea = CreateSafeArea(canvasGo.transform);

        var title = CreateText(safeArea, "Title", "GAME SCENE", 90, Color.white);
        SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 150f), new Vector2(900f, 160f));

        var subtitle = CreateText(safeArea, "Subtitle", "Gameplay comes in Iteration 2", 44, PrimaryColor);
        SetRect(subtitle, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(900f, 100f));

        var backButton = CreateButton(safeArea, "BackButton", "BACK", PrimaryColor, 56);
        SetRect(backButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(460f, 140f));

        var gameUI = canvasGo.AddComponent<GameSceneUI>();
        var so = new SerializedObject(gameUI);
        so.FindProperty("backButton").objectReferenceValue = backButton;
        so.ApplyModifiedPropertiesWithoutUndo();

        CreateBootstrap();
        SaveScene(scene, "Game");
        Debug.Log("Game scene built (Iteration 1)");
    }

    private static bool CheckTmp()
    {
        if (TMP_Settings.defaultFontAsset != null) return true;
        EditorUtility.DisplayDialog("TMP Essentials missing",
            "Import TMP Essentials first: Window > TextMeshPro > Import TMP Essential Resources", "OK");
        return false;
    }

    private static Scene CreateEmptyScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    private static void CreateCamera()
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        var cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = BackgroundColor;
        go.AddComponent<AudioListener>();
        go.transform.position = new Vector3(0f, 0f, -10f);
    }

    private static void CreateEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    private static GameObject CreateCanvas()
    {
        var go = new GameObject("Canvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    private static RectTransform CreateSafeArea(Transform parent)
    {
        var go = new GameObject("SafeArea", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.AddComponent<SafeAreaFitter>();
        return rt;
    }

    private static RectTransform CreateText(Transform parent, string name, string content, float fontSize, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.outlineWidth = 0.15f;
        tmp.outlineColor = new Color32(0, 0, 0, 200);
        tmp.raycastTarget = false;
        return go.GetComponent<RectTransform>();
    }

    private static Button CreateButton(Transform parent, string name, string label, Color color, float fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = color;
        var button = go.AddComponent<Button>();
        go.AddComponent<CanvasGroup>();
        go.AddComponent<ButtonAnimator>();
        var labelRect = CreateText(go.transform, "Label", label, fontSize, Color.white);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        return button;
    }

    private static void SetRect(Component target, Vector2 anchor, Vector2 position, Vector2 size)
    {
        var rt = target.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
    }

    private static void CreateBootstrap()
    {
        var go = new GameObject("Bootstrap");
        go.AddComponent<GameBootstrap>();
    }

    private static void SaveScene(Scene scene, string sceneName)
    {
        if (!Directory.Exists(ScenesFolder))
        {
            Directory.CreateDirectory(ScenesFolder);
            AssetDatabase.Refresh();
        }
        string path = ScenesFolder + "/" + sceneName + ".unity";
        EditorSceneManager.SaveScene(scene, path);
        AddToBuildSettings(path, sceneName == "MainMenu");
    }

    private static void AddToBuildSettings(string path, bool insertFirst)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (scenes.Any(s => s.path == path)) return;
        var entry = new EditorBuildSettingsScene(path, true);
        if (insertFirst) scenes.Insert(0, entry);
        else scenes.Add(entry);
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
