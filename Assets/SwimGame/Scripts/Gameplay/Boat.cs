using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private const float MoveDuration = 0.2f;

    public bool IsMoving { get; private set; }

    private GridManager grid;
    private Vector2Int anchor;
    private int length;
    private Vector2Int direction;
    private bool horizontal;

    public void Init(GridManager gridManager, Vector2Int anchorCell, int boatLength, Vector2Int dir)
    {
        grid = gridManager;
        anchor = anchorCell;
        length = Mathf.Max(1, boatLength);
        direction = dir;
        horizontal = dir.x != 0;
        BuildVisual();
        transform.position = CenterWorld();
        grid.AddBoatCells(GetCells());
    }

    private void BuildVisual()
    {
        var hull = new GameObject("Hull");
        hull.transform.SetParent(transform, false);
        var hullSr = hull.AddComponent<SpriteRenderer>();
        hullSr.sprite = SpriteFactory.Square;
        hullSr.color = new Color32(141, 110, 99, 255);
        hullSr.sortingOrder = 6;
        hull.transform.localScale = horizontal
            ? new Vector3(length * 0.96f, 0.78f, 1f)
            : new Vector3(0.78f, length * 0.96f, 1f);

        var deck = new GameObject("Deck");
        deck.transform.SetParent(transform, false);
        var deckSr = deck.AddComponent<SpriteRenderer>();
        deckSr.sprite = SpriteFactory.Square;
        deckSr.color = new Color32(172, 146, 134, 255);
        deckSr.sortingOrder = 7;
        deck.transform.localScale = horizontal
            ? new Vector3(length * 0.96f - 0.22f, 0.56f, 1f)
            : new Vector3(0.56f, length * 0.96f - 0.22f, 1f);
    }

    private Vector3 CenterWorld()
    {
        Vector3 a = grid.GridToWorld(anchor);
        Vector3 b = grid.GridToWorld(EndCell());
        return (a + b) * 0.5f;
    }

    private Vector2Int EndCell()
    {
        Vector2Int axis = horizontal ? Vector2Int.right : Vector2Int.up;
        return anchor + axis * (length - 1);
    }

    public List<Vector2Int> GetCells()
    {
        var list = new List<Vector2Int>();
        Vector2Int axis = horizontal ? Vector2Int.right : Vector2Int.up;
        for (int i = 0; i < length; i++)
        {
            list.Add(anchor + axis * i);
        }
        return list;
    }

    public void PlanAndMove(Vector2Int playerPos)
    {
        if (TryMove(direction, playerPos)) return;
        direction = -direction;
        TryMove(direction, playerPos);
    }

    private bool TryMove(Vector2Int dir, Vector2Int playerPos)
    {
        Vector2Int lead = dir == Vector2Int.right || dir == Vector2Int.up
            ? EndCell() + dir
            : anchor + dir;

        if (!grid.IsWalkable(lead)) return false;
        if (lead == playerPos) return false;
        if (grid.GetSwimmerAt(lead) != null) return false;

        grid.RemoveBoatCells(GetCells());
        anchor += dir;
        grid.AddBoatCells(GetCells());

        IsMoving = true;
        SplashEffect.Play(grid.GridToWorld(lead), 0.6f);
        transform.DOMove(CenterWorld(), MoveDuration).SetEase(Ease.InOutQuad).OnComplete(() => IsMoving = false);
        return true;
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
