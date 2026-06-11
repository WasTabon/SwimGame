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
    [SerializeField] private ItemManager itemManager;

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
        Refresh();
    }

    private void OnDisable()
    {
        CurrencyManager.OnBalanceChanged -= HandleBalanceChanged;
    }

    private void HandleBalanceChanged(int balance)
    {
        Refresh();
    }

    private void Refresh()
    {
        int balance = CurrencyManager.Balance;
        balanceText.text = "Coins: " + balance;
        flippersCountText.text = "x" + itemManager.GetCount(ItemType.Flippers);
        ballCountText.text = "x" + itemManager.GetCount(ItemType.Ball);
        shieldCountText.text = "x" + itemManager.GetCount(ItemType.Shield);
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
            return;
        }
        ItemInventory.Add(type, 1);
        SoundManager.Instance.PlaySfx(SfxType.Coin);
        HapticManager.Instance.Vibrate();
        itemManager.RefreshCounts();
        Refresh();
    }
}
