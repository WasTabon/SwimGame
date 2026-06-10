using System.Collections.Generic;
using UnityEngine;

public class PatrolSwimmer : SwimmerBase
{
    private List<Vector2Int> waypoints;
    private int targetIndex;

    protected override Color BodyColor => new Color32(230, 126, 34, 255);
    protected override Sprite BodySprite => SpriteFactory.Diamond;

    public void SetRoute(List<Vector2Int> route)
    {
        waypoints = route;
        targetIndex = route.Count > 1 ? 1 : 0;
        Vector2Int target = waypoints[targetIndex];
        Vector2Int dir = new Vector2Int(
            System.Math.Sign(target.x - GridPosition.x),
            System.Math.Sign(target.y - GridPosition.y));
        if (dir.x != 0 && dir.y != 0) dir = new Vector2Int(dir.x, 0);
        if (dir == Vector2Int.zero) dir = Vector2Int.right;
        Direction = dir;
        UpdateArrow(false);
    }

    public override Vector2Int PlanMove(Vector2Int playerPos)
    {
        if (waypoints == null || waypoints.Count < 2) return GridPosition;

        if (GridPosition == waypoints[targetIndex])
        {
            targetIndex = (targetIndex + 1) % waypoints.Count;
        }

        Vector2Int target = waypoints[targetIndex];
        Vector2Int dir = new Vector2Int(
            System.Math.Sign(target.x - GridPosition.x),
            System.Math.Sign(target.y - GridPosition.y));
        if (dir.x != 0 && dir.y != 0) dir = new Vector2Int(dir.x, 0);
        if (dir == Vector2Int.zero) return GridPosition;

        Vector2Int next = GridPosition + dir;
        if (!CanEnter(next)) return GridPosition;

        Direction = dir;
        UpdateArrow();
        return next;
    }
}
