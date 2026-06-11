using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    private static readonly Color StarOnColor = new Color32(245, 166, 35, 255);
    private static readonly Color StarOffColor = new Color32(44, 62, 80, 255);

    [SerializeField] private TextMeshProUGUI movesValueText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameHUD hud;

    private int pendingReward;
    private bool rewardGranted = true;
    private Tween flyDelayTween;

    private void Awake()
    {
        nextButton.onClick.AddListener(OnNext);
        restartButton.onClick.AddListener(OnRestart);
        menuButton.onClick.AddListener(OnMenu);
        foreach (var star in starImages)
        {
            star.sprite = SpriteFactory.Circle;
        }
    }

    public void ShowResult(int moves, int optimalMoves, int stars, bool hasNext, int reward)
    {
        movesValueText.text = "Moves: " + moves + "    Best: " + optimalMoves;
        nextButton.gameObject.SetActive(hasNext);
        pendingReward = reward;
        rewardGranted = reward <= 0;
        rewardText.gameObject.SetActive(reward > 0);
        rewardText.text = "+" + reward;
        Show();

        float starsEnd = 0.35f;
        for (int i = 0; i < starImages.Length; i++)
        {
            var star = starImages[i];
            star.transform.DOKill();
            star.color = StarOffColor;
            star.transform.localScale = Vector3.one;
            if (i < stars)
            {
                star.transform.localScale = Vector3.zero;
                float delay = 0.35f + i * 0.15f;
                starsEnd = delay + 0.3f;
                star.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
                int captured = i;
                DOVirtual.DelayedCall(delay, () =>
                {
                    starImages[captured].color = StarOnColor;
                    SoundManager.Instance.PlaySfx(SfxType.Coin);
                });
            }
        }

        if (reward > 0)
        {
            rewardText.transform.localScale = Vector3.zero;
            rewardText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(starsEnd);
            flyDelayTween?.Kill();
            flyDelayTween = DOVirtual.DelayedCall(starsEnd + 0.45f, StartCoinFly);
        }
    }

    private void StartCoinFly()
    {
        if (rewardGranted) return;
        Transform target = hud != null && hud.CoinIcon != null ? hud.CoinIcon : null;
        if (target == null)
        {
            GrantReward();
            return;
        }
        int coins = Mathf.Clamp(3 + pendingReward / 5, 4, 9);
        CoinFlyEffect.Play(transform.root, rewardText.transform.position, target, coins,
            () => SoundManager.Instance.PlaySfx(SfxType.Coin),
            GrantReward);
    }

    private void GrantReward()
    {
        if (rewardGranted) return;
        rewardGranted = true;
        CurrencyManager.Add(pendingReward);
    }

    private void OnNext()
    {
        flyDelayTween?.Kill();
        GrantReward();
        Hide(levelLoader.NextLevel);
    }

    private void OnRestart()
    {
        flyDelayTween?.Kill();
        GrantReward();
        Hide(levelLoader.RestartLevel);
    }

    private void OnMenu()
    {
        flyDelayTween?.Kill();
        GrantReward();
        TransitionManager.Instance.LoadScene("LevelSelect");
    }

    private void OnDestroy()
    {
        flyDelayTween?.Kill();
        GrantReward();
    }
}
