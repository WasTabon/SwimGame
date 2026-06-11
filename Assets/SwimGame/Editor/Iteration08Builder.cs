using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Iteration08Builder
{
    [MenuItem("SwimGame/Update Game Scene (Iteration 8)")]
    public static void UpdateGameScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/Game.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var gameRoot = GameObject.Find("GameRoot");
        var hud = Object.FindObjectOfType<GameHUD>();
        var itemManager = gameRoot.GetComponent<ItemManager>();
        var winPopup = Object.FindObjectOfType<WinPopup>(true);
        var safeArea = hud.transform.Find("SafeArea");
        var itemBar = safeArea.Find("ItemBar");

        var coinGroup = CreateCoinGroup(safeArea, out var coinIcon, out var coinsText);

        var hudSo = new SerializedObject(hud);
        hudSo.FindProperty("coinsText").objectReferenceValue = coinsText;
        hudSo.FindProperty("coinIcon").objectReferenceValue = coinIcon;
        hudSo.ApplyModifiedPropertiesWithoutUndo();

        SwimGameEditorUtils.SetRect(itemBar.Find("FlippersButton"), new Vector2(0.5f, 0.5f), new Vector2(-315f, 0f), new Vector2(150f, 150f));
        SwimGameEditorUtils.SetRect(itemBar.Find("BallButton"), new Vector2(0.5f, 0.5f), new Vector2(-105f, 0f), new Vector2(150f, 150f));
        SwimGameEditorUtils.SetRect(itemBar.Find("ShieldButton"), new Vector2(0.5f, 0.5f), new Vector2(105f, 0f), new Vector2(150f, 150f));

        var shopButtonTr = itemBar.Find("ShopButton");
        Button shopButton;
        if (shopButtonTr != null)
        {
            shopButton = shopButtonTr.GetComponent<Button>();
        }
        else
        {
            shopButton = SwimGameEditorUtils.CreateButton(itemBar, "ShopButton", "$", SwimGameEditorUtils.AccentColor, 60f);
            SwimGameEditorUtils.SetRect(shopButton.transform, new Vector2(0.5f, 0.5f), new Vector2(310f, 0f), new Vector2(130f, 130f));
        }

        var itemBarUi = itemBar.GetComponent<ItemBarUI>();
        var barSo = new SerializedObject(itemBarUi);
        barSo.FindProperty("shopButton").objectReferenceValue = shopButton;
        barSo.ApplyModifiedPropertiesWithoutUndo();

        ShopPopup shopPopup = Object.FindObjectOfType<ShopPopup>(true);
        if (shopPopup == null)
        {
            shopPopup = BuildShopPopup(winPopup.transform.parent, itemManager);
        }

        var imSo = new SerializedObject(itemManager);
        imSo.FindProperty("shopPopup").objectReferenceValue = shopPopup;
        imSo.ApplyModifiedPropertiesWithoutUndo();

        var window = winPopup.transform.Find("Window");
        var rewardTr = window.Find("RewardText");
        RectTransform rewardText;
        if (rewardTr != null)
        {
            rewardText = rewardTr.GetComponent<RectTransform>();
        }
        else
        {
            bool wasActive = winPopup.gameObject.activeSelf;
            winPopup.gameObject.SetActive(true);
            rewardText = SwimGameEditorUtils.CreateText(window, "RewardText", "+0", 60f, SwimGameEditorUtils.AccentColor);
            SwimGameEditorUtils.SetRect(rewardText, new Vector2(0.5f, 0.5f), new Vector2(0f, -30f), new Vector2(400f, 70f));
            winPopup.gameObject.SetActive(wasActive);
        }

        var winSo = new SerializedObject(winPopup);
        winSo.FindProperty("rewardText").objectReferenceValue = rewardText.GetComponent<TextMeshProUGUI>();
        winSo.FindProperty("hud").objectReferenceValue = hud;
        winSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Game scene updated with economy (Iteration 8)");
    }

    [MenuItem("SwimGame/Update Level Select Scene (Iteration 8)")]
    public static void UpdateLevelSelectScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/LevelSelect.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var ui = Object.FindObjectOfType<LevelSelectUI>();
        var safeArea = ui.transform.Find("SafeArea");

        CreateCoinGroup(safeArea, out var coinIcon, out var coinsText);

        var uiSo = new SerializedObject(ui);
        uiSo.FindProperty("coinsText").objectReferenceValue = coinsText;
        uiSo.FindProperty("coinIcon").objectReferenceValue = coinIcon;
        uiSo.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("LevelSelect scene updated with coin counter (Iteration 8)");
    }

    private static RectTransform CreateCoinGroup(Transform safeArea, out RectTransform coinIcon, out TextMeshProUGUI coinsText)
    {
        var existing = safeArea.Find("CoinGroup");
        if (existing != null)
        {
            coinIcon = existing.Find("CoinIcon").GetComponent<RectTransform>();
            coinsText = existing.Find("CoinsText").GetComponent<TextMeshProUGUI>();
            return existing.GetComponent<RectTransform>();
        }

        var groupGo = new GameObject("CoinGroup", typeof(RectTransform));
        groupGo.transform.SetParent(safeArea, false);
        var groupRt = groupGo.GetComponent<RectTransform>();
        SwimGameEditorUtils.SetRect(groupRt, new Vector2(1f, 1f), new Vector2(-170f, -260f), new Vector2(300f, 70f));

        var iconGo = new GameObject("CoinIcon", typeof(RectTransform));
        iconGo.transform.SetParent(groupGo.transform, false);
        coinIcon = iconGo.GetComponent<RectTransform>();
        SwimGameEditorUtils.SetRect(coinIcon, new Vector2(0f, 0.5f), new Vector2(40f, 0f), new Vector2(56f, 56f));
        var iconImage = iconGo.AddComponent<Image>();
        iconImage.color = SwimGameEditorUtils.AccentColor;
        iconImage.raycastTarget = false;

        var innerGo = new GameObject("Inner", typeof(RectTransform));
        innerGo.transform.SetParent(iconGo.transform, false);
        var innerRt = innerGo.GetComponent<RectTransform>();
        SwimGameEditorUtils.SetRect(innerRt, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(32f, 32f));
        var innerImage = innerGo.AddComponent<Image>();
        innerImage.color = new Color32(255, 214, 130, 255);
        innerImage.raycastTarget = false;

        var textRect = SwimGameEditorUtils.CreateText(groupGo.transform, "CoinsText", "0", 52f, Color.white);
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(84f, 0f);
        textRect.offsetMax = Vector2.zero;
        coinsText = textRect.GetComponent<TextMeshProUGUI>();
        coinsText.alignment = TextAlignmentOptions.MidlineLeft;

        return groupRt;
    }

    private static ShopPopup BuildShopPopup(Transform parent, ItemManager itemManager)
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

        var flippersRow = CreateShopRow(window, "FLIPPERS", ShopPopup.FlippersPrice, 130f, out var flippersCount, out var flippersBuy);
        var ballRow = CreateShopRow(window, "BALL", ShopPopup.BallPrice, -30f, out var ballCount, out var ballBuy);
        var shieldRow = CreateShopRow(window, "SHIELD", ShopPopup.ShieldPrice, -190f, out var shieldCount, out var shieldBuy);

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
        so.FindProperty("itemManager").objectReferenceValue = itemManager;
        so.ApplyModifiedPropertiesWithoutUndo();

        rootGo.SetActive(false);
        return popup;
    }

    private static RectTransform CreateShopRow(RectTransform window, string itemName, int price, float y,
        out TextMeshProUGUI countText, out Button buyButton)
    {
        var rowGo = new GameObject("Row_" + itemName, typeof(RectTransform));
        rowGo.transform.SetParent(window, false);
        var rowRt = rowGo.GetComponent<RectTransform>();
        SwimGameEditorUtils.SetRect(rowRt, new Vector2(0.5f, 0.5f), new Vector2(0f, y), new Vector2(760f, 130f));

        var nameRect = SwimGameEditorUtils.CreateText(rowGo.transform, "Name", itemName, 46f, Color.white);
        SwimGameEditorUtils.SetRect(nameRect, new Vector2(0f, 0.5f), new Vector2(150f, 20f), new Vector2(300f, 60f));
        nameRect.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;

        var countRect = SwimGameEditorUtils.CreateText(rowGo.transform, "Count", "x0", 38f, new Color32(170, 190, 210, 255));
        SwimGameEditorUtils.SetRect(countRect, new Vector2(0f, 0.5f), new Vector2(150f, -32f), new Vector2(300f, 50f));
        countRect.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.MidlineLeft;
        countText = countRect.GetComponent<TextMeshProUGUI>();

        buyButton = SwimGameEditorUtils.CreateButton(rowGo.transform, "BuyButton", price.ToString(), SwimGameEditorUtils.AccentColor, 46f);
        SwimGameEditorUtils.SetRect(buyButton.transform, new Vector2(1f, 0.5f), new Vector2(-120f, 0f), new Vector2(220f, 100f));

        return rowRt;
    }
}
