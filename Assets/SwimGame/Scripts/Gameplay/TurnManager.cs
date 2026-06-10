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
    private bool busy;
    private bool gameOver;

    public bool IsBusy => busy || gameOver;

    public void Setup(List<SwimmerBase> levelSwimmers)
    {
        StopAllCoroutines();
        swimmers.Clear();
        swimmers.AddRange(levelSwimmers);
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
