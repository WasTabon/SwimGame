using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelData SelectedLevel;

    [SerializeField] private LevelData defaultLevel;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private MoveHighlighter highlighter;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InputController inputController;
    [SerializeField] private GameHUD hud;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private WinPopup winPopup;
    [SerializeField] private LosePopup losePopup;
    [SerializeField] private Transform swimmersContainer;

    private readonly List<SwimmerBase> swimmers = new List<SwimmerBase>();
    private LevelData currentLevel;

    private void Start()
    {
        currentLevel = SelectedLevel != null ? SelectedLevel : defaultLevel;
        Debug.Assert(currentLevel != null, "No level assigned to LevelLoader!");

        inputController.OnCellTapped -= HandleCellTapped;
        inputController.OnCellTapped += HandleCellTapped;
        turnManager.OnWin -= HandleWin;
        turnManager.OnWin += HandleWin;
        turnManager.OnLose -= HandleLose;
        turnManager.OnLose += HandleLose;

        BuildLevel();
    }

    private void OnDestroy()
    {
        if (inputController != null) inputController.OnCellTapped -= HandleCellTapped;
        if (turnManager != null)
        {
            turnManager.OnWin -= HandleWin;
            turnManager.OnLose -= HandleLose;
        }
    }

    private void BuildLevel()
    {
        ParsedLevel parsed = currentLevel.Parse();
        gridManager.Build(parsed);
        SpawnSwimmers(parsed);
        player.Init(gridManager, parsed.playerStart);
        highlighter.Init(gridManager);
        highlighter.Refresh(player.GridPosition);
        cameraController.Setup(gridManager, player.transform.position);
        hud.Setup(currentLevel.levelName);
        turnManager.Setup(swimmers);
    }

    private void SpawnSwimmers(ParsedLevel parsed)
    {
        foreach (var swimmer in swimmers)
        {
            if (swimmer != null) Destroy(swimmer.gameObject);
        }
        swimmers.Clear();
        gridManager.ClearSwimmers();

        foreach (var spawn in parsed.swimmers)
        {
            var go = new GameObject("Swimmer_" + spawn.type);
            go.transform.SetParent(swimmersContainer, false);
            SwimmerBase swimmer;
            switch (spawn.type)
            {
                default:
                    swimmer = go.AddComponent<LinearSwimmer>();
                    break;
            }
            swimmer.Init(gridManager, spawn.position, spawn.direction);
            swimmers.Add(swimmer);
        }
    }

    public void RestartLevel()
    {
        BuildLevel();
    }

    public void DoWait()
    {
        turnManager.TryWait();
    }

    private void HandleCellTapped(Vector2Int cell)
    {
        turnManager.TryPlayerMove(cell);
    }

    private void HandleWin()
    {
        winPopup.ShowWithMoves(hud.MovesCount);
    }

    private void HandleLose()
    {
        losePopup.Show();
    }
}
