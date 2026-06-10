using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    [SerializeField] private TextMeshProUGUI movesValueText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        restartButton.onClick.AddListener(OnRestart);
        menuButton.onClick.AddListener(OnMenu);
    }

    public void ShowWithMoves(int moves)
    {
        movesValueText.text = "Moves: " + moves;
        Show();
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
