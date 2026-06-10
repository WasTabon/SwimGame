using DG.Tweening;
using UnityEngine;

public class BallLure : MonoBehaviour
{
    public Vector2Int Cell { get; private set; }

    private Transform visual;
    private Tween idleTween;

    public void Init(GridManager grid, Vector2Int cell, Vector3 fromWorld)
    {
        Cell = cell;
        transform.position = fromWorld;
        BuildVisual();
        transform.DOJump(grid.GridToWorld(cell), 0.7f, 1, 0.45f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SplashEffect.Play(transform.position, 0.7f);
            StartIdle();
        });
    }

    private void BuildVisual()
    {
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform, false);
        visual = visualGo.transform;
        var body = visualGo.AddComponent<SpriteRenderer>();
        body.sprite = SpriteFactory.Circle;
        body.color = new Color32(245, 166, 35, 255);
        body.sortingOrder = 7;
        visual.localScale = Vector3.one * 0.42f;

        var stripeGo = new GameObject("Stripe");
        stripeGo.transform.SetParent(visualGo.transform, false);
        var stripe = stripeGo.AddComponent<SpriteRenderer>();
        stripe.sprite = SpriteFactory.Circle;
        stripe.color = Color.white;
        stripe.sortingOrder = 8;
        stripeGo.transform.localScale = Vector3.one * 0.45f;
        stripeGo.transform.localPosition = new Vector3(0.12f, 0.12f, 0f);
    }

    private void StartIdle()
    {
        idleTween = visual.DOLocalMoveY(0.06f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void PlayDespawn()
    {
        idleTween?.Kill();
        transform.DOKill();
        SplashEffect.Play(transform.position, 0.5f);
        visual.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        idleTween?.Kill();
        transform.DOKill();
        if (visual != null) visual.DOKill();
    }
}
