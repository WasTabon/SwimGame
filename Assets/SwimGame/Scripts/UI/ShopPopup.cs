using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopup : PopupBase
{
    public const int FlippersPrice = 30;
    public const int BallPrice = 40;
    public const int ShieldPrice = 50;

    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private Button flippersBuyButton;
    [SerializeField] private Button ballBuyButton;
    [SerializeField] private Button shieldBuyButton;
    [SerializeField] private TextMeshProUGUI flippersCountText;
    [SerializeField] private TextMeshProUGUI ballCountText;
    [SerializeField] private TextMeshProUGUI shieldCountText;
    [SerializeField] private Button closeButton;

    private GameObject noMoneyToast;
    private RectTransform noMoneyToastRect;
    private Sequence noMoneyToastSeq;

    private void Awake()
    {
        flippersBuyButton.onClick.AddListener(() => Buy(ItemType.Flippers, FlippersPrice));
        ballBuyButton.onClick.AddListener(() => Buy(ItemType.Ball, BallPrice));
        shieldBuyButton.onClick.AddListener(() => Buy(ItemType.Shield, ShieldPrice));
        closeButton.onClick.AddListener(() => Hide());
    }

    private void OnEnable()
    {
        CurrencyManager.OnBalanceChanged += HandleBalanceChanged;
        ItemInventory.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        CurrencyManager.OnBalanceChanged -= HandleBalanceChanged;
        ItemInventory.OnChanged -= Refresh;
        noMoneyToastSeq?.Kill();
        if (noMoneyToast != null)
        {
            Destroy(noMoneyToast);
            noMoneyToast = null;
            noMoneyToastRect = null;
        }
    }

    private void HandleBalanceChanged(int balance)
    {
        Refresh();
    }

    private void Refresh()
    {
        int balance = CurrencyManager.Balance;
        balanceText.text = "Coins: " + balance;
        flippersCountText.text = "x" + ItemInventory.Get(ItemType.Flippers);
        ballCountText.text = "x" + ItemInventory.Get(ItemType.Ball);
        shieldCountText.text = "x" + ItemInventory.Get(ItemType.Shield);
        SetAffordable(flippersBuyButton, balance >= FlippersPrice);
        SetAffordable(ballBuyButton, balance >= BallPrice);
        SetAffordable(shieldBuyButton, balance >= ShieldPrice);
    }

    private void SetAffordable(Button button, bool affordable)
    {
        var group = button.GetComponent<CanvasGroup>();
        if (group != null) group.alpha = affordable ? 1f : 0.5f;
    }

    private void Buy(ItemType type, int price)
    {
        if (!CurrencyManager.TrySpend(price))
        {
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            balanceText.transform.DOKill(true);
            balanceText.transform.DOPunchPosition(new Vector3(12f, 0f, 0f), 0.3f, 12, 0.6f);
            ShowNoMoneyToast();
            return;
        }
        ItemInventory.Add(type, 1);
        SoundManager.Instance.PlaySfx(SfxType.Coin);
        HapticManager.Instance.Vibrate();
    }

    private void ShowNoMoneyToast()
    {
        if (noMoneyToast == null)
        {
            var canvas = GetComponentInParent<Canvas>();

            noMoneyToast = new GameObject("NoMoneyToast", typeof(RectTransform));
            noMoneyToast.transform.SetParent(canvas.transform, false);
            noMoneyToastRect = noMoneyToast.GetComponent<RectTransform>();
            noMoneyToastRect.anchorMin = new Vector2(0.5f, 0.5f);
            noMoneyToastRect.anchorMax = new Vector2(0.5f, 0.5f);
            noMoneyToastRect.pivot = new Vector2(0.5f, 0.5f);
            noMoneyToastRect.anchoredPosition = new Vector2(0f, 360f);
            noMoneyToastRect.sizeDelta = new Vector2(660f, 150f);

            var bg = noMoneyToast.AddComponent<Image>();
            bg.color = new Color32(196, 60, 60, 240);
            bg.raycastTarget = false;

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(noMoneyToast.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20f, 0f);
            textRect.offsetMax = new Vector2(-20f, 0f);
            var text = textGo.AddComponent<TextMeshProUGUI>();
            text.text = "Not enough coins!";
            text.fontSize = 48f;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            text.raycastTarget = false;
        }

        noMoneyToast.transform.SetAsLastSibling();
        noMoneyToastSeq?.Kill();
        noMoneyToastRect.localScale = Vector3.zero;
        noMoneyToastSeq = DOTween.Sequence();
        noMoneyToastSeq.Append(noMoneyToastRect.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        noMoneyToastSeq.AppendInterval(0.9f);
        noMoneyToastSeq.Append(noMoneyToastRect.DOScale(0f, 0.25f).SetEase(Ease.InBack));
    }
}