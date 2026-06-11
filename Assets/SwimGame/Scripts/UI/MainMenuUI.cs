using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private ShopPopup shopPopup;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlay);
        settingsButton.onClick.AddListener(OnSettings);
        shopButton.onClick.AddListener(OnShop);
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
