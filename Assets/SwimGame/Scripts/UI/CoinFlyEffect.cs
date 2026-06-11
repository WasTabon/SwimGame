using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class CoinFlyEffect
{
    private static readonly Color CoinColor = new Color32(245, 166, 35, 255);
    private static readonly Color CoinInnerColor = new Color32(255, 214, 130, 255);

    public static void Play(Transform canvasRoot, Vector3 fromWorld, Transform target, int count,
        System.Action onCoinArrived, System.Action onComplete)
    {
        if (canvasRoot == null || target == null || count <= 0)
        {
            onComplete?.Invoke();
            return;
        }

        int arrived = 0;
        float scale = canvasRoot.lossyScale.x;

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("FlyCoin", typeof(RectTransform));
            go.transform.SetParent(canvasRoot, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(46f, 46f);
            var img = go.AddComponent<Image>();
            img.sprite = SpriteFactory.Circle;
            img.color = CoinColor;
            img.raycastTarget = false;

            var innerGo = new GameObject("Inner", typeof(RectTransform));
            innerGo.transform.SetParent(go.transform, false);
            var innerRt = innerGo.GetComponent<RectTransform>();
            innerRt.sizeDelta = new Vector2(26f, 26f);
            var inner = innerGo.AddComponent<Image>();
            inner.sprite = SpriteFactory.Circle;
            inner.color = CoinInnerColor;
            inner.raycastTarget = false;

            go.transform.position = fromWorld;
            Vector3 burst = fromWorld + (Vector3)(Random.insideUnitCircle * 140f * scale);

            var seq = DOTween.Sequence();
            seq.Append(go.transform.DOMove(burst, 0.22f).SetEase(Ease.OutQuad));
            seq.AppendInterval(0.04f * i);
            seq.Append(go.transform.DOMove(target.position, 0.45f).SetEase(Ease.InQuad));
            seq.Join(go.transform.DOScale(0.55f, 0.45f).SetEase(Ease.InQuad));
            seq.OnComplete(() =>
            {
                onCoinArrived?.Invoke();
                target.DOKill();
                target.localScale = Vector3.one;
                target.DOPunchScale(Vector3.one * 0.2f, 0.18f, 3, 0.6f);
                Object.Destroy(go);
                arrived++;
                if (arrived == count) onComplete?.Invoke();
            });
        }
    }
}
