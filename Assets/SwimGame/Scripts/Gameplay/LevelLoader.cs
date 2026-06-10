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

    private void Start()
    {
        LevelData data = SelectedLevel != null ? SelectedLevel : defaultLevel;
        Debug.Assert(data != null, "No level assigned to LevelLoader!");

        ParsedLevel parsed = data.Parse();
        gridManager.Build(parsed);
        player.Init(gridManager, parsed.playerStart);
        highlighter.Init(gridManager);
        highlighter.Refresh(player.GridPosition);
        cameraController.Setup(gridManager, player.transform.position);
        hud.Setup(data.levelName);

        inputController.OnCellTapped -= HandleCellTapped;
        inputController.OnCellTapped += HandleCellTapped;
        player.OnMoveCompleted -= HandleMoveCompleted;
        player.OnMoveCompleted += HandleMoveCompleted;
    }

    private void OnDestroy()
    {
        if (inputController != null) inputController.OnCellTapped -= HandleCellTapped;
        if (player != null) player.OnMoveCompleted -= HandleMoveCompleted;
    }

    private void HandleCellTapped(Vector2Int cell)
    {
        if (player.IsMoving) return;

        int manhattan = Mathf.Abs(cell.x - player.GridPosition.x) + Mathf.Abs(cell.y - player.GridPosition.y);
        bool validMove = manhattan == 1 && gridManager.IsWalkable(cell);

        if (!validMove)
        {
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            return;
        }

        highlighter.Hide();
        hud.IncrementMoves();
        player.MoveTo(cell);
    }

    private void HandleMoveCompleted()
    {
        highlighter.Refresh(player.GridPosition);
        cameraController.EnsureVisible(player.transform.position);
    }

    public void DoWait()
    {
        if (player.IsMoving) return;
        hud.IncrementMoves();
        player.Wait();
    }
}
