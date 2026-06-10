using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
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

    public bool IsBusy => busy || gameOver;

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
    }

    public void TryPlayerMove(Vector2Int cell)
    {
        if (IsBusy || player.IsMoving) return;

        int manhattan = Mathf.Abs(cell.x - player.GridPosition.x) + Mathf.Abs(cell.y - player.GridPosition.y);
        bool valid = manhattan == 1 && gridManager.IsWalkable(cell);

        if (!valid)
        {
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            return;
        }

        StartCoroutine(TurnRoutine(cell, false));
    }

    public void TryWait()
    {
        if (IsBusy || player.IsMoving) return;
        StartCoroutine(TurnRoutine(player.GridPosition, true));
    }

    private IEnumerator TurnRoutine(Vector2Int targetCell, bool isWait)
    {
        busy = true;
        highlighter.Hide();
        hud.IncrementMoves();

        if (isWait) player.Wait();
        else player.MoveTo(targetCell);

        yield return new WaitWhile(() => player.IsMoving);

        if (!isWait)
        {
            var bumped = gridManager.GetSwimmerAt(player.GridPosition);
            if (bumped != null)
            {
                yield return LoseRoutine(bumped);
                yield break;
            }
            if (player.GridPosition == gridManager.ExitPosition)
            {
                yield return WinRoutine();
                yield break;
            }
            cameraController.EnsureVisible(player.transform.position);
        }

        SwimmerBase killer = null;
        foreach (var swimmer in swimmers)
        {
            swimmer.OnTurnStart();
            bool moved = false;

            for (int step = 0; step < swimmer.StepsPerTurn; step++)
            {
                Vector2Int next = swimmer.PlanMove(player.GridPosition);
                if (next == swimmer.GridPosition) break;

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
            if (gridManager.IsWalkable(dest))
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

        highlighter.Refresh(player.GridPosition);
        busy = false;
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
