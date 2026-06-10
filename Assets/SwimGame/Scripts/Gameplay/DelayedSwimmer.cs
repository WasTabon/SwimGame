using UnityEngine;

public class DelayedSwimmer : LinearSwimmer
{
    private bool resting;

    protected override Color BodyColor => new Color32(26, 188, 156, 255);

    public override Vector2Int PlanMove(Vector2Int playerPos)
    {
        if (resting)
        {
            resting = false;
            SetDim(false);
            return GridPosition;
        }

        Vector2Int result = base.PlanMove(playerPos);
        resting = true;
        SetDim(true);
        return result;
    }
}
