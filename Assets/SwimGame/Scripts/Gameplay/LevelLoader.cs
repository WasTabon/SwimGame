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
                case SwimmerType.Fast:
                    swimmer = go.AddComponent<FastSwimmer>();
                    break;
                case SwimmerType.Delayed:
                    swimmer = go.AddComponent<DelayedSwimmer>();
                    break;
                case SwimmerType.Reactive:
                    swimmer = go.AddComponent<ReactiveSwimmer>();
                    break;
                default:
                    swimmer = go.AddComponent<LinearSwimmer>();
                    break;
            }
            swimmer.Init(gridManager, spawn.position, spawn.direction);
            swimmers.Add(swimmer);
        }

        foreach (var route in parsed.patrols)
        {
            var go = new GameObject("Swimmer_Patrol");
            go.transform.SetParent(swimmersContainer, false);
            var patrol = go.AddComponent<PatrolSwimmer>();

            Vector2Int dir = Vector2Int.right;
            if (route.Count > 1)
            {
                Vector2Int d = route[1] - route[0];
                dir = new Vector2Int(System.Math.Sign(d.x), System.Math.Sign(d.y));
                if (dir.x != 0 && dir.y != 0) dir = new Vector2Int(dir.x, 0);
                if (dir == Vector2Int.zero) dir = Vector2Int.right;
            }

            patrol.Init(gridManager, route[0], dir);
            patrol.SetRoute(route);
            swimmers.Add(patrol);
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
