using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarUI : MonoBehaviour
{
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private Button flippersButton;
    [SerializeField] private Button ballButton;
    [SerializeField] private Button shieldButton;
    [SerializeField] private TextMeshProUGUI flippersCountText;
    [SerializeField] private TextMeshProUGUI ballCountText;
    [SerializeField] private TextMeshProUGUI shieldCountText;

    private void Awake()
    {
        flippersButton.onClick.AddListener(() => itemManager.OnItemButton(ItemType.Flippers));
        ballButton.onClick.AddListener(() => itemManager.OnItemButton(ItemType.Ball));
        shieldButton.onClick.AddListener(() => itemManager.OnItemButton(ItemType.Shield));
    }

    public void SetCounts(int flippers, int ball, int shield)
    {
        flippersCountText.text = "x" + flippers;
        ballCountText.text = "x" + ball;
        shieldCountText.text = "x" + shield;
        SetButtonDim(flippersButton, flippers > 0);
        SetButtonDim(ballButton, ball > 0);
        SetButtonDim(shieldButton, shield > 0);
    }

    private void SetButtonDim(Button button, bool hasItems)
    {
        var group = button.GetComponent<CanvasGroup>();
        if (group != null) group.alpha = hasItems ? 1f : 0.5f;
    }

    public void SetActiveItem(ItemType? type)
    {
        Highlight(flippersButton.transform, type == ItemType.Flippers);
        Highlight(ballButton.transform, type == ItemType.Ball);
        Highlight(shieldButton.transform, type == ItemType.Shield);
    }

    private void Highlight(Transform target, bool active)
    {
        target.DOKill();
        target.DOScale(active ? 1.12f : 1f, 0.15f).SetEase(Ease.OutQuad);
    }
}
