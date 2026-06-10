using UnityEngine;

public class LinearSwimmer : SwimmerBase
{
    public override Vector2Int PlanMove(Vector2Int playerPos)
    {
        Vector2Int next = GridPosition + Direction;
        if (CanEnter(next)) return next;

        Direction = -Direction;
        UpdateArrow();

        next = GridPosition + Direction;
        if (CanEnter(next)) return next;

        return GridPosition;
    }
}
