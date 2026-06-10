using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float BaseScale = 0.72f;
    private const float MoveDuration = 0.22f;

    [SerializeField] private SpriteRenderer visualRenderer;

    public Vector2Int GridPosition { get; private set; }
    public bool IsMoving { get; private set; }

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
        transform.DOKill();
        KillVisualTweens();
        IsMoving = false;
        transform.position = grid.GridToWorld(start);
        visual.localPosition = Vector3.zero;
        visual.localScale = Vector3.one * BaseScale;
        visualRenderer.color = Color.white;
        StartIdle();
    }

    private void KillVisualTweens()
    {
        idleTween?.Kill();
        squashSequence?.Kill();
        visual.DOKill();
        visualRenderer.DOKill();
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
        });
    }

    public void PlayWinAnimation()
    {
        KillVisualTweens();
        visual.localPosition = Vector3.zero;
        visual.localScale = Vector3.one * BaseScale;

        var seq = DOTween.Sequence();
        seq.Append(visual.DOScale(Vector3.one * BaseScale * 1.2f, 0.15f).SetEase(Ease.OutBack));
        seq.Append(visual.DOLocalMoveY(0.4f, 0.22f).SetEase(Ease.OutQuad));
        seq.Append(visual.DOLocalMoveY(0f, 0.18f).SetEase(Ease.InQuad));
        seq.AppendCallback(() => SplashEffect.Play(transform.position, 0.8f));
        seq.Append(visual.DOLocalMoveY(0.22f, 0.16f).SetEase(Ease.OutQuad));
        seq.Append(visual.DOLocalMoveY(0f, 0.14f).SetEase(Ease.InQuad));
        seq.Join(visual.DOScale(Vector3.one * BaseScale, 0.2f).SetEase(Ease.OutBack));
    }

    public void PlayDeathAnimation()
    {
        KillVisualTweens();
        transform.DOKill();
        visual.localPosition = Vector3.zero;
        visualRenderer.DOColor(new Color32(231, 76, 60, 255), 0.12f);

        var seq = DOTween.Sequence();
        seq.Append(visual.DOScale(Vector3.one * BaseScale * 1.3f, 0.1f).SetEase(Ease.OutQuad));
        seq.Append(visual.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InBack));
    }
}
