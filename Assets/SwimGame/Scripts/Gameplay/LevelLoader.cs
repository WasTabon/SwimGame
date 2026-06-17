using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private const int BaseReward = 10;
    private const int StarReward = 5;
    private const int RepeatReward = 5;

    public static int SelectedLevelIndex = -1;
    public static LevelData SelectedLevel;

    [SerializeField] private LevelDatabase database;
    [SerializeField] private LevelData defaultLevel;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private MoveHighlighter highlighter;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InputController inputController;
    [SerializeField] private GameHUD hud;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private TutorialController tutorialController;
    [SerializeField] private WinPopup winPopup;
    [SerializeField] private LosePopup losePopup;
    [SerializeField] private Transform swimmersContainer;
    [SerializeField] private Sprite linearSprite;
    [SerializeField] private Sprite fastSprite;
    [SerializeField] private Sprite delayedSprite;
    [SerializeField] private Sprite reactiveSprite;
    [SerializeField] private Sprite patrolSprite;

    private readonly List<SwimmerBase> swimmers = new List<SwimmerBase>();
    private readonly List<Boat> boats = new List<Boat>();
    private readonly List<TogglePlatform> platforms = new List<TogglePlatform>();
    private LevelData currentLevel;

    private bool HasDatabaseLevel =>
        database != null && SelectedLevelIndex >= 0 && SelectedLevelIndex < database.levels.Count;

    private void Start()
    {
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

    private void ResolveCurrentLevel()
    {
        if (HasDatabaseLevel)
        {
            currentLevel = database.levels[SelectedLevelIndex];
            return;
        }
        currentLevel = SelectedLevel != null ? SelectedLevel : defaultLevel;
    }

    private void BuildLevel()
    {
        ResolveCurrentLevel();
        Debug.Assert(currentLevel != null, "No level assigned to LevelLoader!");

        ParsedLevel parsed = currentLevel.Parse();
        gridManager.Build(parsed);
        SpawnSwimmers(parsed);
        SpawnDynamics(parsed);
        player.Init(gridManager, parsed.playerStart);
        highlighter.Init(gridManager);
        highlighter.Refresh(player.GridPosition);
        cameraController.Setup(gridManager, player.transform.position);
        hud.Setup(currentLevel.levelName, currentLevel.optimalMoves);
        turnManager.Setup(swimmers, boats, platforms);
        itemManager.ResetForLevel();
        if (tutorialController != null)
        {
            tutorialController.Begin(HasDatabaseLevel ? SelectedLevelIndex : -1);
        }
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
            swimmer.OverrideSprite = SpriteForType(spawn.type);
            swimmer.Init(gridManager, spawn.position, spawn.direction);
            swimmers.Add(swimmer);
        }

        foreach (var route in parsed.patrols)
        {
            var go = new GameObject("Swimmer_Patrol");
            go.transform.SetParent(swimmersContainer, false);
            var patrol = go.AddComponent<PatrolSwimmer>();
            patrol.OverrideSprite = patrolSprite;

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

    private Sprite SpriteForType(SwimmerType type)
    {
        switch (type)
        {
            case SwimmerType.Fast: return fastSprite;
            case SwimmerType.Delayed: return delayedSprite;
            case SwimmerType.Reactive: return reactiveSprite;
            default: return linearSprite;
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

    public void NextLevel()
    {
        if (HasDatabaseLevel && SelectedLevelIndex + 1 < database.levels.Count)
        {
            SelectedLevelIndex++;
            BuildLevel();
            return;
        }
        TransitionManager.Instance.LoadScene("LevelSelect");
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
        int stars = ProgressManager.CalculateStars(hud.MovesCount, currentLevel.optimalMoves);
        bool hasNext = false;
        int reward = 0;
        if (HasDatabaseLevel)
        {
            int previousStars = ProgressManager.GetStars(SelectedLevelIndex);
            ProgressManager.SetStars(SelectedLevelIndex, stars);
            hasNext = SelectedLevelIndex + 1 < database.levels.Count;
            reward = stars > previousStars ? BaseReward + stars * StarReward : RepeatReward;
        }
        winPopup.ShowResult(hud.MovesCount, currentLevel.optimalMoves, stars, hasNext, reward);
    }

    private void HandleLose()
    {
        losePopup.Show();
    }
}