using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(CanvasGroup))]
public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private CanvasGroup canvasGroup;
    private Tween scaleTween;

    private void Awake()
    {
        button = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        button.transition = Selectable.Transition.None;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        scaleTween?.Kill();
        scaleTween = transform.DOScale(0.95f, 0.05f).SetEase(Ease.OutQuad);
        SoundManager.Instance.PlaySfx(SfxType.Tap);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        scaleTween?.Kill();
        scaleTween = transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
        canvasGroup.alpha = value ? 1f : 0.5f;
        if (!value)
        {
            scaleTween?.Kill();
            transform.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }
}
