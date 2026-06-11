using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private TurnManager turnManager;

    private static readonly Dictionary<int, string> Solutions = new Dictionary<int, string>
    {
        { 0, "D D D D R R R" },
        { 1, "R R R D D L L L D D D R R R" },
        { 2, "R R R D D L D D D R" }
    };

    private readonly List<Vector2Int> path = new List<Vector2Int>();
    private int step;
    private bool active;
    private GameObject marker;

    private void OnEnable()
    {
        turnManager.OnTurnEnded += HandleTurnEnded;
        turnManager.OnWin += Stop;
        turnManager.OnLose += Stop;
    }

    private void OnDisable()
    {
        turnManager.OnTurnEnded -= HandleTurnEnded;
        turnManager.OnWin -= Stop;
        turnManager.OnLose -= Stop;
    }

    public void Begin(int levelIndex)
    {
        Stop();
        if (!Solutions.TryGetValue(levelIndex, out string solution)) return;
        if (ProgressManager.GetStars(levelIndex) > 0) return;

        path.Clear();
        Vector2Int pos = player.GridPosition;
        foreach (var token in solution.Split(' '))
        {
            pos += ToDirection(token);
            path.Add(pos);
        }
        step = 0;
        active = true;
        ShowMarker();
    }

    public void Stop()
    {
        active = false;
        HideMarker();
    }

    private void HandleTurnEnded()
    {
        if (!active) return;
        if (player.GridPosition == path[step])
        {
            step++;
            if (step >= path.Count)
            {
                Stop();
                return;
            }
            ShowMarker();
        }
        else
        {
            Stop();
        }
    }

    private void ShowMarker()
    {
        HideMarker();
        marker = new GameObject("TutorialMarker");
        marker.transform.SetParent(transform, false);
        marker.transform.position = gridManager.GridToWorld(path[step]);
        var sr = marker.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Circle;
        sr.color = new Color(0.96f, 0.65f, 0.14f, 0.55f);
        sr.sortingOrder = 6;
        marker.transform.localScale = Vector3.one * 0.45f;
        marker.transform.DOScale(0.8f, 0.55f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void HideMarker()
    {
        if (marker == null) return;
        marker.transform.DOKill();
        Destroy(marker);
        marker = null;
    }

    private static Vector2Int ToDirection(string token)
    {
        switch (token)
        {
            case "U": return Vector2Int.up;
            case "D": return Vector2Int.down;
            case "L": return Vector2Int.left;
            case "R": return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }
}
