using DG.Tweening;
using UnityEngine;

public abstract class SwimmerBase : MonoBehaviour
{
    protected const float BaseScale = 0.66f;
    private const float MoveDuration = 0.18f;

    public Vector2Int GridPosition { get; protected set; }
    public Vector2Int Direction { get; protected set; }
    public bool IsMoving { get; private set; }

    protected GridManager grid;
    private Transform visual;
    private SpriteRenderer visualRenderer;
    private Transform arrow;
    private Tween idleTween;
    private Sequence squashSequence;

    protected virtual Color BodyColor => new Color32(231, 76, 60, 255);

    public void Init(GridManager gridManager, Vector2Int start, Vector2Int direction)
    {
        grid = gridManager;
        GridPosition = start;
        Direction = direction;
        transform.position = grid.GridToWorld(start);
        BuildVisual();
        UpdateArrow(false);
        StartIdle();
        grid.RegisterSwimmer(this, start);
    }

    private void BuildVisual()
    {
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform, false);
        visual = visualGo.transform;
        visualRenderer = visualGo.AddComponent<SpriteRenderer>();
        visualRenderer.sprite = SpriteFactory.Circle;
        visualRenderer.color = BodyColor;
        visualRenderer.sortingOrder = 8;
        visual.localScale = Vector3.one * BaseScale;

        var arrowGo = new GameObject("Arrow");
        arrowGo.transform.SetParent(visualGo.transform, false);
        var arrowRenderer = arrowGo.AddComponent<SpriteRenderer>();
        arrowRenderer.sprite = SpriteFactory.Triangle;
        arrowRenderer.color = new Color(1f, 1f, 1f, 0.95f);
        arrowRenderer.sortingOrder = 9;
        arrow = arrowGo.transform;
        arrow.localScale = Vector3.one * 0.42f;
    }

    protected void UpdateArrow(bool animated = true)
    {
        Vector3 dir = new Vector3(Direction.x, Direction.y, 0f);
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg - 90f;
        if (animated)
        {
            arrow.DOKill();
            arrow.DOLocalMove(dir * 0.5f, 0.12f).SetEase(Ease.OutQuad);
            arrow.DOLocalRotate(new Vector3(0f, 0f, angle), 0.12f).SetEase(Ease.OutQuad);
        }
        else
        {
            arrow.localPosition = dir * 0.5f;
            arrow.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void StartIdle()
    {
        idleTween?.Kill();
        visual.localPosition = Vector3.zero;
        idleTween = visual.DOLocalMoveY(0.05f, 1.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(Random.Range(0f, 0.5f));
    }

    public abstract Vector2Int PlanMove(Vector2Int playerPos);

    protected bool CanEnter(Vector2Int cell)
    {
        return grid.IsWalkable(cell) && grid.GetSwimmerAt(cell) == null;
    }

    public void MoveTo(Vector2Int cell)
    {
        grid.MoveSwimmer(this, GridPosition, cell);
        Vector2Int dir = cell - GridPosition;
        GridPosition = cell;
        IsMoving = true;

        SplashEffect.Play(transform.position, 0.5f);

        idleTween?.Kill();
        visual.localPosition = Vector3.zero;

        Vector3 squash = dir.x != 0
            ? new Vector3(BaseScale * 1.18f, BaseScale * 0.84f, 1f)
            : new Vector3(BaseScale * 0.84f, BaseScale * 1.18f, 1f);
        squashSequence?.Kill();
        squashSequence = DOTween.Sequence();
        squashSequence.Append(visual.DOScale(squash, 0.07f).SetEase(Ease.OutQuad));
        squashSequence.Append(visual.DOScale(Vector3.one * BaseScale, 0.14f).SetEase(Ease.OutBack));

        transform.DOMove(grid.GridToWorld(cell), MoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            IsMoving = false;
            StartIdle();
        });
    }

    private void OnDestroy()
    {
        transform.DOKill();
        idleTween?.Kill();
        squashSequence?.Kill();
        if (visual != null) visual.DOKill();
        if (arrow != null) arrow.DOKill();
    }
}
