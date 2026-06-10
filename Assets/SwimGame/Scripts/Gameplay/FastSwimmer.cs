using UnityEngine;

public class FastSwimmer : LinearSwimmer
{
    public override int StepsPerTurn => 2;

    protected override Color BodyColor => new Color32(155, 89, 182, 255);

    protected override void BuildDecorations(Transform visualParent)
    {
        var go = new GameObject("Arrow2");
        go.transform.SetParent(arrow, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Triangle;
        sr.color = new Color(1f, 1f, 1f, 0.65f);
        sr.sortingOrder = 9;
        go.transform.localPosition = new Vector3(0f, -0.85f, 0f);
        go.transform.localScale = Vector3.one * 0.8f;
    }
}
