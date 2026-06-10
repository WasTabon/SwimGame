using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private const int BallRadius = 3;
    private const int BallThrowRange = 4;
    private const int BallDuration = 3;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private MoveHighlighter highlighter;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameHUD hud;

    public event System.Action OnWin;
    public event System.Action OnLose;

    private readonly List<SwimmerBase> swimmers = new List<SwimmerBase>();
    private readonly List<Boat> boats = new List<Boat>();
    private readonly List<TogglePlatform> platforms = new List<TogglePlatform>();
    private bool busy;
    private bool gameOver;
    private int shieldTurnsLeft;
    private BallLure ballLure;
    private int ballTurnsLeft;

    public bool IsBusy => busy || gameOver;
    public bool ShieldActive => shieldTurnsLeft > 0;

    public void Setup(List<SwimmerBase> levelSwimmers, List<Boat> levelBoats, List<TogglePlatform> levelPlatforms)
    {
        StopAllCoroutines();
        swimmers.Clear();
        swimmers.AddRange(levelSwimmers);
        boats.Clear();
        boats.AddRange(levelBoats);
        platforms.Clear();
        platforms.AddRange(levelPlatforms);
        busy = false;
        gameOver = false;
        shieldTurnsLeft = 0;
        player.SetShield(false);
        if (ballLure != null)
        {
            Destroy(ballLure.gameObject);
            ballLure = null;
        }
        ballTurnsLeft = 0;
    }

    public void TryPlayerMove(Vector2Int cell)
    {
        if (IsBusy || player.IsMoving) return;

        int manhattan = Manhattan(cell, player.GridPosition);
        bool valid = manhattan == 1 && gridManager.IsWalkable(cell);
        if (valid && ShieldActive && gridManager.GetSwimmerAt(cell) != null) valid = false;

        if (!valid)
        {
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            return;
        }

        StartCoroutine(MoveRoutine(cell, false));
    }

    public void TryWait()
    {
        if (IsBusy || player.IsMoving) return;
        StartCoroutine(MoveRoutine(player.GridPosition, true));
    }

    public bool TryDash(Vector2Int via, Vector2Int target)
    {
        if (IsBusy || player.IsMoving) return false;

        Vector2Int d1 = via - player.GridPosition;
        Vector2Int d2 = target - via;
        if (d1 != d2 || Mathf.Abs(d1.x) + Mathf.Abs(d1.y) != 1) return false;
        if (!gridManager.IsWalkable(via) || !gridManager.IsWalkable(target)) return false;
        if (ShieldActive && (gridManager.GetSwimmerAt(via) != null || gridManager.GetSwimmerAt(target) != null)) return false;

        StartCoroutine(DashRoutine(via, target));
        return true;
    }

    public bool TryThrowBall(Vector2Int cell)
    {
        if (IsBusy || player.IsMoving) return false;
        if (cell == player.GridPosition) return false;
        if (Manhattan(cell, player.GridPosition) > BallThrowRange) return false;
        if (!gridManager.IsWalkable(cell)) return false;

        StartCoroutine(ThrowBallRoutine(cell));
        return true;
    }

    public bool TryUseShield()
    {
        if (IsBusy || player.IsMoving) return false;
        if (ShieldActive) return false;

        StartCoroutine(ShieldRoutine());
        return true;
    }

    private IEnumerator MoveRoutine(Vector2Int targetCell, bool isWait)
    {
        BeginTurn();

        if (isWait) player.Wait();
        else player.MoveTo(targetCell);

        yield return new WaitWhile(() => player.IsMoving);

        if (!isWait)
        {
            yield return CheckPlayerCell();
            if (gameOver) yield break;
            cameraController.EnsureVisible(player.transform.position);
        }

        yield return WorldPhase();
        if (gameOver) yield break;
        EndTurn();
    }

    private IEnumerator DashRoutine(Vector2Int via, Vector2Int target)
    {
        BeginTurn();

        player.MoveTo(via);
        yield return new WaitWhile(() => player.IsMoving);
        yield return CheckPlayerCell();
        if (gameOver) yield break;

        player.MoveTo(target);
        yield return new WaitWhile(() => player.IsMoving);
        yield return CheckPlayerCell();
        if (gameOver) yield break;

        cameraController.EnsureVisible(player.transform.position);
        yield return WorldPhase();
        if (gameOver) yield break;
        EndTurn();
    }

    private IEnumerator ThrowBallRoutine(Vector2Int cell)
    {
        BeginTurn();

        if (ballLure != null) Destroy(ballLure.gameObject);
        var go = new GameObject("BallLure");
        ballLure = go.AddComponent<BallLure>();
        ballLure.Init(gridManager, cell, player.transform.position);
        ballTurnsLeft = BallDuration;

        SoundManager.Instance.PlaySfx(SfxType.Tap);
        player.Wait();
        yield return new WaitForSeconds(0.45f);
        yield return new WaitWhile(() => player.IsMoving);

        yield return WorldPhase();
        if (gameOver) yield break;
        EndTurn();
    }

    private IEnumerator ShieldRoutine()
    {
        BeginTurn();

        shieldTurnsLeft = 2;
        player.SetShield(true);
        SoundManager.Instance.PlaySfx(SfxType.Coin);
        player.Wait();
        yield return new WaitWhile(() => player.IsMoving);

        yield return WorldPhase();
        if (gameOver) yield break;
        EndTurn();
    }

    private void BeginTurn()
    {
        busy = true;
        highlighter.Hide();
        hud.IncrementMoves();
    }

    private void EndTurn()
    {
        if (shieldTurnsLeft > 0)
        {
            shieldTurnsLeft--;
            if (shieldTurnsLeft == 0) player.SetShield(false);
        }
        highlighter.Refresh(player.GridPosition);
        busy = false;
    }

    private IEnumerator CheckPlayerCell()
    {
        var bumped = gridManager.GetSwimmerAt(player.GridPosition);
        if (bumped != null && !ShieldActive)
        {
            yield return LoseRoutine(bumped);
            yield break;
        }
        if (player.GridPosition == gridManager.ExitPosition)
        {
            yield return WinRoutine();
        }
    }

    private IEnumerator WorldPhase()
    {
        SwimmerBase killer = null;
        foreach (var swimmer in swimmers)
        {
            swimmer.OnTurnStart();
            bool attracted = ballTurnsLeft > 0 && ballLure != null
                && Manhattan(swimmer.GridPosition, ballLure.Cell) <= BallRadius;
            bool moved = false;

            if (attracted)
            {
                Vector2Int next = PlanAttractedMove(swimmer);
                if (next != swimmer.GridPosition && !(ShieldActive && next == player.GridPosition))
                {
                    swimmer.FaceDirection(next - swimmer.GridPosition);
                    swimmer.MoveTo(next);
                    moved = true;
                    if (next == player.GridPosition) killer = swimmer;
                }
            }
            else
            {
                for (int step = 0; step < swimmer.StepsPerTurn; step++)
                {
                    Vector2Int next = swimmer.PlanMove(player.GridPosition);
                    if (next == swimmer.GridPosition) break;
                    if (ShieldActive && next == player.GridPosition) break;

                    swimmer.MoveTo(next);
                    moved = true;

                    if (next == player.GridPosition)
                    {
                        killer = swimmer;
                        break;
                    }

                    if (swimmer.StepsPerTurn > 1)
                    {
                        yield return new WaitWhile(() => swimmer.IsMoving);
                    }
                }
            }

            if (killer != null) break;
            if (moved) yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitWhile(AnySwimmerMoving);

        if (killer != null)
        {
            yield return LoseRoutine(killer);
            yield break;
        }

        foreach (var boat in boats)
        {
            boat.PlanAndMove(player.GridPosition);
            if (boat.IsMoving) yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitWhile(AnyBoatMoving);

        Vector2Int playerCurrent = gridManager.GetCurrent(player.GridPosition);
        if (playerCurrent != Vector2Int.zero)
        {
            Vector2Int dest = player.GridPosition + playerCurrent;
            bool shieldBlocked = ShieldActive && gridManager.GetSwimmerAt(dest) != null;
            if (gridManager.IsWalkable(dest) && !shieldBlocked)
            {
                var hit = gridManager.GetSwimmerAt(dest);
                player.MoveTo(dest);
                yield return new WaitWhile(() => player.IsMoving);

                if (hit != null)
                {
                    yield return LoseRoutine(hit);
                    yield break;
                }
                if (player.GridPosition == gridManager.ExitPosition)
                {
                    yield return WinRoutine();
                    yield break;
                }
                cameraController.EnsureVisible(player.transform.position);
            }
        }

        SwimmerBase currentKiller = null;
        foreach (var swimmer in swimmers)
        {
            Vector2Int flow = gridManager.GetCurrent(swimmer.GridPosition);
            if (flow == Vector2Int.zero) continue;

            Vector2Int dest = swimmer.GridPosition + flow;
            if (!gridManager.IsWalkable(dest)) continue;
            if (gridManager.GetSwimmerAt(dest) != null) continue;
            if (ShieldActive && dest == player.GridPosition) continue;

            swimmer.MoveTo(dest);
            if (dest == player.GridPosition)
            {
                currentKiller = swimmer;
                break;
            }
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitWhile(AnySwimmerMoving);

        if (currentKiller != null)
        {
            yield return LoseRoutine(currentKiller);
            yield break;
        }

        foreach (var platform in platforms)
        {
            platform.Tick(player.GridPosition);
        }

        if (ballTurnsLeft > 0)
        {
            ballTurnsLeft--;
            if (ballTurnsLeft == 0 && ballLure != null)
            {
                ballLure.PlayDespawn();
                ballLure = null;
            }
        }
    }

    private Vector2Int PlanAttractedMove(SwimmerBase swimmer)
    {
        Vector2Int pos = swimmer.GridPosition;
        Vector2Int delta = ballLure.Cell - pos;
        if (delta == Vector2Int.zero) return pos;

        Vector2Int primary;
        Vector2Int secondary;
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y) && delta.x != 0)
        {
            primary = new Vector2Int(System.Math.Sign(delta.x), 0);
            secondary = new Vector2Int(0, System.Math.Sign(delta.y));
        }
        else
        {
            primary = new Vector2Int(0, System.Math.Sign(delta.y));
            secondary = new Vector2Int(System.Math.Sign(delta.x), 0);
        }

        foreach (var dir in new[] { primary, secondary })
        {
            if (dir == Vector2Int.zero) continue;
            Vector2Int next = pos + dir;
            if (gridManager.IsWalkable(next) && gridManager.GetSwimmerAt(next) == null) return next;
        }
        return pos;
    }

    private int Manhattan(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private bool AnySwimmerMoving()
    {
        foreach (var swimmer in swimmers)
        {
            if (swimmer != null && swimmer.IsMoving) return true;
        }
        return false;
    }

    private bool AnyBoatMoving()
    {
        foreach (var boat in boats)
        {
            if (boat != null && boat.IsMoving) return true;
        }
        return false;
    }

    private IEnumerator WinRoutine()
    {
        gameOver = true;
        SoundManager.Instance.PlaySfx(SfxType.Win);
        player.PlayWinAnimation();
        yield return new WaitForSeconds(0.9f);
        OnWin?.Invoke();
    }

    private IEnumerator LoseRoutine(SwimmerBase killer)
    {
        gameOver = true;
        SoundManager.Instance.PlaySfx(SfxType.Lose);
        HapticManager.Instance.Vibrate();
        CollisionEffect.Play(player.transform.position);
        cameraController.Shake();
        player.PlayDeathAnimation();
        if (killer != null) killer.PlayKillPunch();
        yield return new WaitForSeconds(0.85f);
        OnLose?.Invoke();
    }
}
