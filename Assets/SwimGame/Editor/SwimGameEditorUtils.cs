using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SwimGameEditorUtils
{
    public const string ScenesFolder = "Assets/SwimGame/Scenes";

    public static readonly Color BackgroundColor = new Color32(26, 26, 46, 255);
    public static readonly Color PrimaryColor = new Color32(74, 144, 226, 255);
    public static readonly Color AccentColor = new Color32(245, 166, 35, 255);
    public static readonly Color PanelColor = new Color32(22, 33, 62, 235);

    public static bool CheckTmp()
    {
        if (TMP_Settings.defaultFontAsset != null) return true;
        EditorUtility.DisplayDialog("TMP Essentials missing",
            "Import TMP Essentials first: Window > TextMeshPro > Import TMP Essential Resources", "OK");
        return false;
    }

    public static Scene CreateEmptyScene()
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    public static GameObject CreateCamera()
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
        return go;
    }

    public static void CreateEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    public static GameObject CreateCanvas()
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

    public static RectTransform CreateSafeArea(Transform parent)
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

    public static RectTransform CreatePanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var image = go.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = color;
        return go.GetComponent<RectTransform>();
    }

    public static RectTransform CreateText(Transform parent, string name, string content, float fontSize, Color color)
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

    public static Button CreateButton(Transform parent, string name, string label, Color color, float fontSize)
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

    public static void SetRect(Component target, Vector2 anchor, Vector2 position, Vector2 size)
    {
        SetRect(target, anchor, position, size, new Vector2(0.5f, 0.5f));
    }

    public static void SetRect(Component target, Vector2 anchor, Vector2 position, Vector2 size, Vector2 pivot)
    {
        var rt = target.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
    }

    public static void StretchHorizontally(Component target, float anchorY, float height, float offsetY, Vector2 pivot)
    {
        var rt = target.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, anchorY);
        rt.anchorMax = new Vector2(1f, anchorY);
        rt.pivot = pivot;
        rt.sizeDelta = new Vector2(0f, height);
        rt.anchoredPosition = new Vector2(0f, offsetY);
    }

    public static void CreateBootstrap()
    {
        var go = new GameObject("Bootstrap");
        go.AddComponent<GameBootstrap>();
    }

    public static void SaveScene(Scene scene, string sceneName)
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

    public static void AddToBuildSettings(string path, bool insertFirst)
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (scenes.Any(s => s.path == path)) return;
        var entry = new EditorBuildSettingsScene(path, true);
        if (insertFirst) scenes.Insert(0, entry);
        else scenes.Add(entry);
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    public static void EnsureFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
