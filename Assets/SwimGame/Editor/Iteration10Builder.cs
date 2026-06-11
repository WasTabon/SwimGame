using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration10Builder
{
    [MenuItem("SwimGame/Update Main Menu Scene (Iteration 10)")]
    public static void UpdateMainMenuScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/MainMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var menuUi = Object.FindObjectOfType<MainMenuUI>();
        var canvas = Object.FindObjectOfType<Canvas>();

        var shopPopup = Object.FindObjectOfType<ShopPopup>(true);
        if (shopPopup == null)
        {
            shopPopup = BuildShopPopup(canvas.transform);
        }

        var so = new SerializedObject(menuUi);
        so.FindProperty("shopPopup").objectReferenceValue = shopPopup;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Main menu updated with shop popup (Iteration 10)");
    }

    private static ShopPopup BuildShopPopup(Transform parent)
    {
        var rootGo = new GameObject("ShopPopup", typeof(RectTransform));
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
        SwimGameEditorUtils.SetRect(window, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(850f, 980f));

        var title = SwimGameEditorUtils.CreateText(window, "Title", "SHOP", 80f, SwimGameEditorUtils.AccentColor);
        SwimGameEditorUtils.SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0f, 380f), new Vector2(700f, 100f));

        var balance = SwimGameEditorUtils.CreateText(window, "Balance", "Coins: 0", 50f, Color.white);
        SwimGameEditorUtils.SetRect(balance, new Vector2(0.5f, 0.5f), new Vector2(0f, 280f), new Vector2(700f, 70f));

        CreateShopRow(window, "FLIPPERS", ShopPopup.FlippersPrice, 130f, out var flippersCount, out var flippersBuy);
        CreateShopRow(window, "BALL", ShopPopup.BallPrice, -30f, out var ballCount, out var ballBuy);
        CreateShopRow(window, "SHIELD", ShopPopup.ShieldPrice, -190f, out var shieldCount, out var shieldBuy);

        var closeButton = SwimGameEditorUtils.CreateButton(window, "CloseButton", "CLOSE", SwimGameEditorUtils.PrimaryColor, 48f);
        SwimGameEditorUtils.SetRect(closeButton.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -380f), new Vector2(420f, 110f));

        var popup = rootGo.AddComponent<ShopPopup>();
        var so = new SerializedObject(popup);
        so.FindProperty("backdrop").objectReferenceValue = backdrop;
        so.FindProperty("window").objectReferenceValue = window;
        so.FindProperty("balanceText").objectReferenceValue = balance.GetComponent<TextMeshProUGUI>();
        so.FindProperty("flippersBuyButton").objectReferenceValue = flippersBuy;
        so.FindProperty("ballBuyButton").objectReferenceValue = ballBuy;
        so.FindProperty("shieldBuyButton").objectReferenceValue = shieldBuy;
        so.FindProperty("flippersCountText").objectReferenceValue = flippersCount;
        so.FindProperty("ballCountText").objectReferenceValue = ballCount;
        so.FindProperty("shieldCountText").objectReferenceValue = shieldCount;
        so.FindProperty("closeButton").objectReferenceValue = closeButton;
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);
        return popup;
    }

    private static void CreateShopRow(RectTransform window, string itemName, int price, float y,
        out TextMeshProUGUI countText, out Button buyButton)
    {
        var rowGo = new GameObject("Row_" + itemName, typeof(RectTransform));
        rowGo.transform.SetParent(window, false);
        SwimGameEditorUtils.SetRect(rowGo.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(760f, 130f));

        var nameRect = SwimGameEditorUtils.CreateText(rowGo.transform, "Name", itemName, 46f, Color.white);
        SwimGameEditorUtils.SetRect(nameRect, new Vector2(0f, 0.5f), new Vector2(150f, 20f), new Vector2(300f, 60f));
        nameRect.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

        var countRect = SwimGameEditorUtils.CreateText(rowGo.transform, "Count", "x0", 38f, new Color32(170, 190, 210, 255));
        SwimGameEditorUtils.SetRect(countRect, new Vector2(0f, 0.5f), new Vector2(150f, -32f), new Vector2(300f, 50f));
        countRect.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
        countText = countRect.GetComponent<TextMeshProUGUI>();

        buyButton = SwimGameEditorUtils.CreateButton(rowGo.transform, "BuyButton", price.ToString(), SwimGameEditorUtils.AccentColor, 46f);
        SwimGameEditorUtils.SetRect(buyButton.transform, new Vector2(1f, 0.5f), new Vector2(-120f, 0f), new Vector2(220f, 100f));
    }
}
