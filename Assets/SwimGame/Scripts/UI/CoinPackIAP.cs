using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class CoinPackIAP : MonoBehaviour
{
    public const string ProductId = "com.coinspacksmall.inapp";
    public const int CoinsAmount = 500;

    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RectTransform coinTarget;

    public void OnProductFetched(Product product)
    {
        if (product == null || product.metadata == null) return;
        priceText.text = product.metadata.localizedPriceString;
    }

    public void OnOrderConfirmed(ConfirmedOrder order)
    {
        GrantCoins();
    }

    public void OnPurchaseFailed(FailedOrder failedOrder)
    {
        FailFeedback(failedOrder.FailureReason + " — " + failedOrder.Details);
    }

    public void OnPurchaseCompleteLegacy(Product product)
    {
        GrantCoins();
    }

    public void OnPurchaseFailedLegacy()
    {
        FailFeedback("legacy event, reason unavailable");
    }

    private void GrantCoins()
    {
        CurrencyManager.Add(CoinsAmount);
        SoundManager.Instance.PlaySfx(SfxType.Coin);
        HapticManager.Instance.Vibrate();
        if (coinTarget != null)
        {
            CoinFlyEffect.Play(transform.root, transform.position, coinTarget, 8,
                () => SoundManager.Instance.PlaySfx(SfxType.Coin), null);
        }
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * 0.15f, 0.25f, 4, 0.6f);
    }

    private void FailFeedback(string reason)
    {
        SoundManager.Instance.PlaySfx(SfxType.Deny);
        Debug.LogWarning("IAP purchase failed: " + reason);
        transform.DOKill(true);
        transform.DOPunchPosition(new Vector3(12f, 0f, 0f), 0.3f, 12, 0.6f);
    }
}