using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    private static readonly Color StarOnColor = new Color32(245, 166, 35, 255);
    private static readonly Color StarOffColor = new Color32(44, 62, 80, 255);

    [SerializeField] private TextMeshProUGUI movesValueText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private LevelLoader levelLoader;

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

    public void ShowResult(int moves, int optimalMoves, int stars, bool hasNext)
    {
        movesValueText.text = "Moves: " + moves + "    Best: " + optimalMoves;
        nextButton.gameObject.SetActive(hasNext);
        Show();

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
                star.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
                int captured = i;
                DOVirtual.DelayedCall(delay, () =>
                {
                    starImages[captured].color = StarOnColor;
                    SoundManager.Instance.PlaySfx(SfxType.Coin);
                });
            }
        }
    }

    private void OnNext()
    {
        Hide(levelLoader.NextLevel);
    }

    private void OnRestart()
    {
        Hide(levelLoader.RestartLevel);
    }

    private void OnMenu()
    {
        TransitionManager.Instance.LoadScene("LevelSelect");
    }
}
