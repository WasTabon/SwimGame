using UnityEngine;
using UnityEngine.UI;

public class PausePopup : PopupBase
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private SettingsPopup settingsPopup;
    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => Hide());
        restartButton.onClick.AddListener(OnRestart);
        settingsButton.onClick.AddListener(OnSettings);
        menuButton.onClick.AddListener(OnMenu);
    }

    private void OnRestart()
    {
        Hide(levelLoader.RestartLevel);
    }

    private void OnSettings()
    {
        settingsPopup.Show();
    }

    private void OnMenu()
    {
        TransitionManager.Instance.LoadScene("LevelSelect");
    }
}
