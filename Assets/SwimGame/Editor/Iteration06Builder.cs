using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration06Builder
{
    private static readonly Color FlippersColor = new Color32(23, 162, 184, 255);
    private static readonly Color BallColor = new Color32(245, 166, 35, 255);
    private static readonly Color ShieldColor = new Color32(133, 193, 233, 255);

    [MenuItem("SwimGame/Update Game Scene (Iteration 6)")]
    public static void UpdateGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var gameRoot = GameObject.Find("GameRoot");
        if (gameRoot == null)
        {
            Debug.LogError("GameRoot not found. Run previous iteration builders first.");
            return;
        }

        var gridManager = gameRoot.GetComponent<GridManager>();
        var loader = gameRoot.GetComponent<LevelLoader>();
        var turnManager = gameRoot.GetComponent<TurnManager>();
        var playerController = Object.FindObjectOfType<PlayerController>();
        var hud = Object.FindObjectOfType<GameHUD>();
        var safeArea = hud.transform.Find("SafeArea");

        var itemManager = gameRoot.GetComponent<ItemManager>();
        if (itemManager == null) itemManager = gameRoot.AddComponent<ItemManager>();

        ItemBarUI itemBar;
        var itemBarTr = safeArea.Find("ItemBar");
        if (itemBarTr != null)
        {
            itemBar = itemBarTr.GetComponent<ItemBarUI>();
        }
        else
        {
            var barGo = new GameObject("ItemBar", typeof(RectTransform));
            barGo.transform.SetParent(safeArea, false);
            var barRt = barGo.GetComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0.5f, 0f);
            barRt.anchorMax = new Vector2(0.5f, 0f);
            barRt.pivot = new Vector2(0.5f, 0.5f);
            barRt.anchoredPosition = new Vector2(0f, 380f);
            barRt.sizeDelta = new Vector2(760f, 160f);
            itemBar = barGo.AddComponent<ItemBarUI>();

            var flippersButton = CreateItemButton(barGo.transform, "FlippersButton", "F", FlippersColor, -250f, out var flippersCount);
            var ballButton = CreateItemButton(barGo.transform, "BallButton", "B", BallColor, 0f, out var ballCount);
            var shieldButton = CreateItemButton(barGo.transform, "ShieldButton", "S", ShieldColor, 250f, out var shieldCount);

            var barSo = new SerializedObject(itemBar);
            barSo.FindProperty("itemManager").objectReferenceValue = itemManager;
            barSo.FindProperty("flippersButton").objectReferenceValue = flippersButton;
            barSo.FindProperty("ballButton").objectReferenceValue = ballButton;
            barSo.FindProperty("shieldButton").objectReferenceValue = shieldButton;
            barSo.FindProperty("flippersCountText").objectReferenceValue = flippersCount;
            barSo.FindProperty("ballCountText").objectReferenceValue = ballCount;
            barSo.FindProperty("shieldCountText").objectReferenceValue = shieldCount;
            barSo.ApplyModifiedPropertiesWithoutUndo();
        }

        var imSo = new SerializedObject(itemManager);
        imSo.FindProperty("gridManager").objectReferenceValue = gridManager;
        imSo.FindProperty("player").objectReferenceValue = playerController;
        imSo.FindProperty("turnManager").objectReferenceValue = turnManager;
        imSo.FindProperty("itemBar").objectReferenceValue = itemBar;
        imSo.ApplyModifiedPropertiesWithoutUndo();

        var loaderSo = new SerializedObject(loader);
        loaderSo.FindProperty("itemManager").objectReferenceValue = itemManager;
        loaderSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with items (Iteration 6)");
    }

    private static Button CreateItemButton(Transform parent, string name, string letter, Color color, float x,
        out TextMeshProUGUI countText)
    {
        var button = SwimGameEditorUtils.CreateButton(parent, name, letter, color, 64f);
        SwimGameEditorUtils.SetRect(button.transform, new Vector2(0.5f, 0.5f), new Vector2(x, 0f), new Vector2(150f, 150f));

        var countRect = SwimGameEditorUtils.CreateText(button.transform, "Count", "x0", 36f, Color.white);
        SwimGameEditorUtils.SetRect(countRect, new Vector2(1f, 0f), new Vector2(-44f, 32f), new Vector2(90f, 50f));
        countText = countRect.GetComponent<TextMeshProUGUI>();

        return button;
    }
}
