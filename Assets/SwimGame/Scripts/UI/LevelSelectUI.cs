using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    private static readonly Color StarOnColor = new Color32(245, 166, 35, 255);
    private static readonly Color StarOffColor = new Color32(26, 26, 46, 255);

    [SerializeField] private LevelDatabase database;
    [SerializeField] private Button backButton;
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private RectTransform coinIcon;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Sprite lockedSprite;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBack);
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
        UpdateCoins(CurrencyManager.Balance);
        CurrencyManager.OnBalanceChanged += UpdateCoins;
    }

    private void OnDisable()
    {
        CurrencyManager.OnBalanceChanged -= UpdateCoins;
    }

    private void UpdateCoins(int balance)
    {
        if (coinsText != null) coinsText.text = balance.ToString();
    }

    private void Start()
    {
        BuildGrid();
    }

    private void BuildGrid()
    {
        for (int i = gridContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(gridContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < database.levels.Count; i++)
        {
            CreateCell(i);
        }
    }

    private void CreateCell(int index)
    {
        bool unlocked = ProgressManager.IsUnlocked(index);
        int stars = ProgressManager.GetStars(index);

        var cellGo = new GameObject("LevelCell_" + (index + 1), typeof(RectTransform));
        cellGo.transform.SetParent(gridContainer, false);

        var bg = cellGo.AddComponent<Image>();
        bg.sprite = unlocked ? unlockedSprite : lockedSprite;
        bg.color = Color.white;

        var button = cellGo.AddComponent<Button>();
        button.targetGraphic = bg;
        cellGo.AddComponent<ButtonAnimator>();

        if (unlocked)
        {
            var numberGo = new GameObject("Number", typeof(RectTransform));
            numberGo.transform.SetParent(cellGo.transform, false);
            var numberRt = numberGo.GetComponent<RectTransform>();
            numberRt.anchorMin = new Vector2(0f, 0.3f);
            numberRt.anchorMax = new Vector2(1f, 1f);
            numberRt.offsetMin = Vector2.zero;
            numberRt.offsetMax = Vector2.zero;
            var numberText = numberGo.AddComponent<TextMeshProUGUI>();
            numberText.text = (index + 1).ToString();
            numberText.fontSize = 64f;
            numberText.fontStyle = FontStyles.Bold;
            numberText.alignment = TextAlignmentOptions.Center;
            numberText.color = Color.white;
            numberText.raycastTarget = false;

            for (int s = 0; s < 3; s++)
            {
                var starGo = new GameObject("Star_" + s, typeof(RectTransform));
                starGo.transform.SetParent(cellGo.transform, false);
                var starRt = starGo.GetComponent<RectTransform>();
                starRt.anchorMin = new Vector2(0.5f, 0f);
                starRt.anchorMax = new Vector2(0.5f, 0f);
                starRt.sizeDelta = new Vector2(34f, 34f);
                starRt.anchoredPosition = new Vector2((s - 1) * 44f, 40f);
                var starImage = starGo.AddComponent<Image>();
                starImage.sprite = SpriteFactory.Circle;
                starImage.color = s < stars ? StarOnColor : StarOffColor;
                starImage.raycastTarget = false;
            }

            int captured = index;
            button.onClick.AddListener(() => OnLevelClicked(captured));
        }
        else
        {
            button.interactable = false;
        }

        cellGo.transform.localScale = Vector3.zero;
        cellGo.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(index * 0.02f);
    }

    private void OnLevelClicked(int index)
    {
        LevelLoader.SelectedLevelIndex = index;
        TransitionManager.Instance.LoadScene("Game");
    }

    private void OnBack()
    {
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}