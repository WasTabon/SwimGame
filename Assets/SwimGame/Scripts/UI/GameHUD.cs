using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private RectTransform coinIcon;
    [SerializeField] private Button backButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private PausePopup pausePopup;
    [SerializeField] private Button waitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private LevelLoader levelLoader;

    private int moves;
    private int optimalMoves = 10;
    private int displayedCoins;
    private Tween coinsTween;

    public int MovesCount => moves;
    public RectTransform CoinIcon => coinIcon;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBack);
        waitButton.onClick.AddListener(OnWait);
        restartButton.onClick.AddListener(OnRestart);
        pauseButton.onClick.AddListener(OnPause);
        if (coinIcon != null)
        {
            foreach (var img in coinIcon.GetComponentsInChildren<Image>())
            {
                img.sprite = SpriteFactory.Circle;
            }
        }
    }

    private void OnEnable()
    {
        displayedCoins = CurrencyManager.Balance;
        coinsText.text = displayedCoins.ToString();
        CurrencyManager.OnBalanceChanged += HandleBalanceChanged;
    }

    private void OnDisable()
    {
        CurrencyManager.OnBalanceChanged -= HandleBalanceChanged;
        coinsTween?.Kill();
    }

    private void HandleBalanceChanged(int newBalance)
    {
        coinsTween?.Kill();
        coinsTween = DOVirtual.Int(displayedCoins, newBalance, 0.6f, v =>
        {
            displayedCoins = v;
            coinsText.text = v.ToString();
        }).SetEase(Ease.OutQuad);
    }

    public void Setup(string levelName, int levelOptimalMoves)
    {
        levelNameText.text = levelName;
        moves = 0;
        optimalMoves = levelOptimalMoves;
        UpdateMovesText();
    }

    public void IncrementMoves()
    {
        moves++;
        UpdateMovesText();
        movesText.transform.DOKill(true);
        movesText.transform.DOPunchScale(Vector3.one * 0.12f, 0.18f, 1, 0.5f);
    }

    private void UpdateMovesText()
    {
        movesText.text = "Moves: " + moves + " / " + optimalMoves;
        if (moves <= optimalMoves) movesText.color = Color.white;
        else if (moves <= optimalMoves + 3) movesText.color = new Color32(245, 166, 35, 255);
        else movesText.color = new Color32(231, 76, 60, 255);
    }

    private void OnPause()
    {
        pausePopup.Show();
    }

    private void OnBack()
    {
        TransitionManager.Instance.LoadScene("LevelSelect");
    }

    private void OnWait()
    {
        levelLoader.DoWait();
    }

    private void OnRestart()
    {
        levelLoader.RestartLevel();
    }
}
