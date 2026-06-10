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
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private WinPopup winPopup;
    [SerializeField] private LosePopup losePopup;
    [SerializeField] private Transform swimmersContainer;

    private readonly List<SwimmerBase> swimmers = new List<SwimmerBase>();
    private readonly List<Boat> boats = new List<Boat>();
    private readonly List<TogglePlatform> platforms = new List<TogglePlatform>();
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
        SpawnDynamics(parsed);
        player.Init(gridManager, parsed.playerStart);
        highlighter.Init(gridManager);
        highlighter.Refresh(player.GridPosition);
        cameraController.Setup(gridManager, player.transform.position);
        hud.Setup(currentLevel.levelName);
        turnManager.Setup(swimmers, boats, platforms);
        itemManager.ResetForLevel();
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

    private void SpawnDynamics(ParsedLevel parsed)
    {
        foreach (var boat in boats)
        {
            if (boat != null) Destroy(boat.gameObject);
        }
        boats.Clear();

        foreach (var platform in platforms)
        {
            if (platform != null) Destroy(platform.gameObject);
        }
        platforms.Clear();

        foreach (var spawn in parsed.boats)
        {
            var go = new GameObject("Boat");
            go.transform.SetParent(swimmersContainer, false);
            var boat = go.AddComponent<Boat>();
            boat.Init(gridManager, spawn.anchor, spawn.length, spawn.direction);
            boats.Add(boat);
        }

        foreach (var spawn in parsed.platforms)
        {
            var go = new GameObject("Platform");
            go.transform.SetParent(swimmersContainer, false);
            var platform = go.AddComponent<TogglePlatform>();
            platform.Init(gridManager, spawn.cell, spawn.openTurns, spawn.closedTurns, spawn.offset);
            platforms.Add(platform);
        }
    }

    public void RestartLevel()
    {
        BuildLevel();
    }

    public void DoWait()
    {
        itemManager.CancelAim();
        turnManager.TryWait();
    }

    private void HandleCellTapped(Vector2Int cell)
    {
        if (itemManager.IsAiming)
        {
            itemManager.HandleAimTap(cell);
            return;
        }
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
