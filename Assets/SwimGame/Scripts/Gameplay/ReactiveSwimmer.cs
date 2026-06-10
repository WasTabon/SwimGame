using UnityEngine;

public class ReactiveSwimmer : LinearSwimmer
{
    private const int DetectRange = 3;

    protected override Color BodyColor => new Color32(241, 196, 15, 255);

    protected override void BuildDecorations(Transform visualParent)
    {
        var eyeGo = new GameObject("Eye");
        eyeGo.transform.SetParent(visualParent, false);
        var eye = eyeGo.AddComponent<SpriteRenderer>();
        eye.sprite = SpriteFactory.Circle;
        eye.color = Color.white;
        eye.sortingOrder = 9;
        eyeGo.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        eyeGo.transform.localScale = Vector3.one * 0.34f;

        var pupilGo = new GameObject("Pupil");
        pupilGo.transform.SetParent(eyeGo.transform, false);
        var pupil = pupilGo.AddComponent<SpriteRenderer>();
        pupil.sprite = SpriteFactory.Circle;
        pupil.color = new Color32(26, 26, 46, 255);
        pupil.sortingOrder = 10;
        pupilGo.transform.localScale = Vector3.one * 0.45f;
        pupilGo.transform.localPosition = new Vector3(0.08f, 0f, 0f);
    }

    public override Vector2Int PlanMove(Vector2Int playerPos)
    {
        Vector2Int delta = playerPos - GridPosition;
        bool aligned = (delta.x == 0) != (delta.y == 0);
        int dist = Mathf.Abs(delta.x) + Mathf.Abs(delta.y);

        if (aligned && dist <= DetectRange && HasClearLine(playerPos))
        {
            Vector2Int dir = new Vector2Int(System.Math.Sign(delta.x), System.Math.Sign(delta.y));
            Direction = dir;
            UpdateArrow();
            Vector2Int next = GridPosition + dir;
            if (CanEnter(next)) return next;
            return GridPosition;
        }

        return base.PlanMove(playerPos);
    }

    private bool HasClearLine(Vector2Int target)
    {
        Vector2Int dir = new Vector2Int(System.Math.Sign(target.x - GridPosition.x), System.Math.Sign(target.y - GridPosition.y));
        Vector2Int cur = GridPosition + dir;
        while (cur != target)
        {
            if (!grid.IsWalkable(cur)) return false;
            cur += dir;
        }
        return true;
    }
}
