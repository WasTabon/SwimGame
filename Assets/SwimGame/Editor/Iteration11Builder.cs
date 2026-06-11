using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public static class Iteration11Builder
{
    [MenuItem("SwimGame/Update Main Menu Scene (Iteration 11)")]
    public static void UpdateMainMenuScene()
    {
        if (!SwimGameEditorUtils.CheckTmp()) return;

        string scenePath = SwimGameEditorUtils.ScenesFolder + "/MainMenu.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        var menuUi = Object.FindObjectOfType<MainMenuUI>();
        var canvas = Object.FindObjectOfType<Canvas>();
        var safeArea = FindSafeArea(canvas.transform);

        var coinGroup = CreateCoinGroup(safeArea, out var coinIcon, out var coinsText);

        var menuSo = new SerializedObject(menuUi);
        menuSo.FindProperty("coinsText").objectReferenceValue = coinsText;
        menuSo.FindProperty("coinIcon").objectReferenceValue = coinIcon;
        menuSo.ApplyModifiedPropertiesWithoutUndo();

        var packTr = safeArea.Find("CoinPackButton");
        if (packTr == null)
        {
            var packButton = SwimGameEditorUtils.CreateButton(safeArea, "CoinPackButton", "+" + CoinPackIAP.CoinsAmount + " COINS",
                SwimGameEditorUtils.AccentColor, 44f);
            SwimGameEditorUtils.SetRect(packButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(520f, 130f));

            var labelRect = packButton.transform.Find("Label").GetComponent<RectTransform>();
            labelRect.offsetMin = new Vector2(0f, 28f);

            var priceRect = SwimGameEditorUtils.CreateText(packButton.transform, "PriceText", "...", 30f, new Color32(255, 240, 210, 255));
            priceRect.anchorMin = new Vector2(0f, 0f);
            priceRect.anchorMax = new Vector2(1f, 0f);
            priceRect.pivot = new Vector2(0.5f, 0f);
            priceRect.anchoredPosition = new Vector2(0f, 6f);
            priceRect.sizeDelta = new Vector2(0f, 36f);

            var iapButton = packButton.gameObject.AddComponent<CodelessIAPButton>();
            iapButton.productId = CoinPackIAP.ProductId;
            iapButton.button = packButton;
            iapButton.automaticallyConfirmTransaction = true;

            var pack = packButton.gameObject.AddComponent<CoinPackIAP>();
            var packSo = new SerializedObject(pack);
            packSo.FindProperty("priceText").objectReferenceValue = priceRect.GetComponent<TextMeshProUGUI>();
            packSo.FindProperty("coinTarget").objectReferenceValue = coinIcon;
            packSo.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(iapButton);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Main menu updated with IAP coin pack (Iteration 11)");
    }

    private static Transform FindSafeArea(Transform canvas)
    {
        var safeArea = canvas.Find("SafeArea");
        if (safeArea != null) return safeArea;
        return canvas;
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
        SwimGameEditorUtils.SetRect(groupRt, new Vector2(1f, 1f), new Vector2(-170f, -140f), new Vector2(300f, 70f));

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
}
