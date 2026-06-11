using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private ShopPopup shopPopup;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private RectTransform coinIcon;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlay);
        settingsButton.onClick.AddListener(OnSettings);
        shopButton.onClick.AddListener(OnShop);
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

    private void OnPlay()
    {
        TransitionManager.Instance.LoadScene("LevelSelect");
    }

    private void OnSettings()
    {
        if (settingsPopup != null) settingsPopup.Show();
    }

    private void OnShop()
    {
        if (shopPopup != null) shopPopup.Show();
    }
}
