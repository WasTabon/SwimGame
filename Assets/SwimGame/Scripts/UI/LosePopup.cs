using UnityEngine;
using UnityEngine.UI;

public class LosePopup : PopupBase
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestart);
        menuButton.onClick.AddListener(OnMenu);
    }

    private void OnRestart()
    {
        Hide(levelLoader.RestartLevel);
    }

    private void OnMenu()
    {
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
