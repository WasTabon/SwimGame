using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private Image backdrop;
    [SerializeField] private RectTransform window;

    private bool hiding;

    public void Show()
    {
        hiding = false;
        gameObject.SetActive(true);
        backdrop.color = new Color(0f, 0f, 0f, 0f);
        backdrop.DOFade(0.6f, 0.25f).SetEase(Ease.OutQuad);
        window.localScale = Vector3.zero;
        window.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
    }

    public void Hide(System.Action onHidden = null)
    {
        if (hiding) return;
        hiding = true;
        backdrop.DOFade(0f, 0.22f).SetEase(Ease.OutQuad);
        window.DOScale(0f, 0.26f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            hiding = false;
            onHidden?.Invoke();
        });
    }
}
