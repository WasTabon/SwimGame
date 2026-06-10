using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button waitButton;
    [SerializeField] private LevelLoader levelLoader;

    private int moves;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBack);
        waitButton.onClick.AddListener(OnWait);
    }

    public void Setup(string levelName)
    {
        levelNameText.text = levelName;
        moves = 0;
        movesText.text = "Moves: 0";
    }

    public void IncrementMoves()
    {
        moves++;
        movesText.text = "Moves: " + moves;
        movesText.transform.DOKill(true);
        movesText.transform.DOPunchScale(Vector3.one * 0.12f, 0.18f, 1, 0.5f);
    }

    private void OnBack()
    {
        TransitionManager.Instance.LoadScene("MainMenu");
    }

    private void OnWait()
    {
        levelLoader.DoWait();
    }
}
