using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float BaseScale = 0.72f;
    private const float MoveDuration = 0.22f;

    [SerializeField] private SpriteRenderer visualRenderer;

    public Vector2Int GridPosition { get; private set; }
    public bool IsMoving { get; private set; }

    public event System.Action OnMoveCompleted;

    private GridManager grid;
    private Transform visual;
    private Tween idleTween;
    private Sequence squashSequence;

    private void Awake()
    {
        visual = visualRenderer.transform;
        visualRenderer.sprite = SpriteFactory.Circle;
        visualRenderer.color = Color.white;
        visualRenderer.sortingOrder = 10;
        visual.localScale = Vector3.one * BaseScale;
    }

    public void Init(GridManager gridManager, Vector2Int start)
    {
        grid = gridManager;
        GridPosition = start;
        transform.position = grid.GridToWorld(start);
        StartIdle();
    }

    private void StartIdle()
    {
        idleTween?.Kill();
        visual.localPosition = Vector3.zero;
        idleTween = visual.DOLocalMoveY(0.06f, 1.1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void MoveTo(Vector2Int cell)
    {
        if (IsMoving) return;
        IsMoving = true;
        Vector2Int from = GridPosition;
        GridPosition = cell;

        SoundManager.Instance.PlaySfx(SfxType.Step);
        SplashEffect.Play(transform.position);

        idleTween?.Kill();
        visual.localPosition = Vector3.zero;

        Vector2Int dir = cell - from;
        Vector3 squash = dir.x != 0
            ? new Vector3(BaseScale * 1.2f, BaseScale * 0.82f, 1f)
            : new Vector3(BaseScale * 0.82f, BaseScale * 1.2f, 1f);

        squashSequence?.Kill();
        squashSequence = DOTween.Sequence();
        squashSequence.Append(visual.DOScale(squash, 0.08f).SetEase(Ease.OutQuad));
        squashSequence.Append(visual.DOScale(Vector3.one * BaseScale, 0.16f).SetEase(Ease.OutBack));

        transform.DOMove(grid.GridToWorld(cell), MoveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            IsMoving = false;
            StartIdle();
            OnMoveCompleted?.Invoke();
        });
    }

    public void Wait()
    {
        if (IsMoving) return;
        IsMoving = true;

        SoundManager.Instance.PlaySfx(SfxType.Step);
        SplashEffect.Play(transform.position);

        idleTween?.Kill();
        visual.localPosition = Vector3.zero;

        squashSequence?.Kill();
        squashSequence = DOTween.Sequence();
        squashSequence.Append(visual.DOScale(Vector3.one * BaseScale * 0.88f, 0.1f).SetEase(Ease.OutQuad));
        squashSequence.Append(visual.DOScale(Vector3.one * BaseScale, 0.18f).SetEase(Ease.OutBack));
        squashSequence.OnComplete(() =>
        {
            IsMoving = false;
            StartIdle();
            OnMoveCompleted?.Invoke();
        });
    }
}
