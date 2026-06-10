using DG.Tweening;
using UnityEngine;

public class TogglePlatform : MonoBehaviour
{
    private GridManager grid;
    private Vector2Int cell;
    private int openTurns;
    private int closedTurns;
    private bool closed;
    private int turnsLeft;
    private SpriteRenderer sr;
    private Tween blinkTween;

    public void Init(GridManager gridManager, Vector2Int platformCell, int open, int close, int offset)
    {
        grid = gridManager;
        cell = platformCell;
        openTurns = Mathf.Max(1, open);
        closedTurns = Mathf.Max(1, close);
        transform.position = grid.GridToWorld(cell);
        BuildVisual();

        closed = false;
        turnsLeft = openTurns;
        for (int i = 0; i < offset; i++)
        {
            turnsLeft--;
            if (turnsLeft <= 0)
            {
                closed = !closed;
                turnsLeft = closed ? closedTurns : openTurns;
            }
        }

        grid.SetPlatformBlocked(cell, closed);
        sr.color = SetAlpha(sr.color, closed ? 1f : 0f);
        UpdateIndicator();
    }

    private void BuildVisual()
    {
        var go = new GameObject("Deck");
        go.transform.SetParent(transform, false);
        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Square;
        sr.color = new Color32(189, 195, 199, 255);
        sr.sortingOrder = 3;
        go.transform.localScale = Vector3.one * 0.94f;
    }

    public void Tick(Vector2Int playerPos)
    {
        turnsLeft--;
        if (turnsLeft <= 0)
        {
            if (closed)
            {
                closed = false;
                turnsLeft = openTurns;
                grid.SetPlatformBlocked(cell, false);
                AnimateSink();
            }
            else
            {
                bool occupied = playerPos == cell || grid.GetSwimmerAt(cell) != null;
                if (occupied)
                {
                    turnsLeft = 1;
                }
                else
                {
                    closed = true;
                    turnsLeft = closedTurns;
                    grid.SetPlatformBlocked(cell, true);
                    AnimateRise();
                }
            }
        }
        UpdateIndicator();
    }

    private void AnimateRise()
    {
        KillTweens();
        transform.localScale = Vector3.one * 0.6f;
        sr.color = SetAlpha(sr.color, 0f);
        sr.DOFade(1f, 0.25f);
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        SplashEffect.Play(transform.position, 0.7f);
    }

    private void AnimateSink()
    {
        KillTweens();
        transform.localScale = Vector3.one;
        sr.DOFade(0f, 0.25f);
        SplashEffect.Play(transform.position, 0.7f);
    }

    private void UpdateIndicator()
    {
        if (turnsLeft == 1)
        {
            if (closed)
            {
                blinkTween?.Kill();
                blinkTween = sr.DOFade(0.45f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
            else
            {
                sr.DOKill();
                sr.DOFade(0.25f, 0.25f);
            }
        }
        else
        {
            if (!closed && sr.color.a > 0.01f && sr.color.a < 0.9f)
            {
                sr.DOKill();
                sr.DOFade(0f, 0.2f);
            }
        }
    }

    private void KillTweens()
    {
        blinkTween?.Kill();
        sr.DOKill();
        transform.DOKill();
    }

    private Color SetAlpha(Color c, float a)
    {
        c.a = a;
        return c;
    }

    private void OnDestroy()
    {
        blinkTween?.Kill();
        if (sr != null) sr.DOKill();
        transform.DOKill();
    }
}
