using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum ItemType { Flippers, Ball, Shield }

public class ItemManager : MonoBehaviour
{
    private const int BallThrowRange = 4;

    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private ItemBarUI itemBar;
    [SerializeField] private ShopPopup shopPopup;
    [SerializeField] private bool debugGrantItems = false;
    [SerializeField] private int debugGrantCount = 3;

    private readonly Dictionary<ItemType, int> sessionCounts = new Dictionary<ItemType, int>();
    private ItemType? aiming;
    private readonly List<GameObject> aimMarkers = new List<GameObject>();

    public bool IsAiming => aiming.HasValue;

    public void ResetForLevel()
    {
        CancelAim();
        if (debugGrantItems)
        {
            sessionCounts[ItemType.Flippers] = debugGrantCount;
            sessionCounts[ItemType.Ball] = debugGrantCount;
            sessionCounts[ItemType.Shield] = debugGrantCount;
        }
        PushCounts();
    }

    public int GetCount(ItemType type)
    {
        if (debugGrantItems)
        {
            sessionCounts.TryGetValue(type, out int c);
            return c;
        }
        return ItemInventory.Get(type);
    }

    private bool ConsumeOne(ItemType type)
    {
        if (debugGrantItems)
        {
            int c = GetCount(type);
            if (c <= 0) return false;
            sessionCounts[type] = c - 1;
            return true;
        }
        return ItemInventory.TryConsume(type);
    }

    public void OpenShop()
    {
        if (turnManager.IsBusy) return;
        CancelAim();
        if (shopPopup != null) shopPopup.Show();
    }

    public void RefreshCounts()
    {
        PushCounts();
    }

    public void OnItemButton(ItemType type)
    {
        if (turnManager.IsBusy) return;

        if (GetCount(type) <= 0)
        {
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            CancelAim();
            if (shopPopup != null) shopPopup.Show();
            return;
        }

        if (type == ItemType.Shield)
        {
            CancelAim();
            if (turnManager.TryUseShield()) Consume(ItemType.Shield);
            else SoundManager.Instance.PlaySfx(SfxType.Deny);
            return;
        }

        if (aiming == type)
        {
            CancelAim();
            return;
        }

        CancelAim();
        aiming = type;
        itemBar.SetActiveItem(type);
        if (type == ItemType.Flippers) ShowDashMarkers();
        else ShowBallMarkers();
    }

    public void HandleAimTap(Vector2Int cell)
    {
        if (aiming == ItemType.Flippers)
        {
            Vector2Int delta = cell - player.GridPosition;
            bool straightTwo = (delta.x == 0) != (delta.y == 0)
                && Mathf.Abs(delta.x) + Mathf.Abs(delta.y) == 2;
            if (straightTwo)
            {
                Vector2Int via = player.GridPosition + new Vector2Int(delta.x / 2, delta.y / 2);
                if (turnManager.TryDash(via, cell))
                {
                    Consume(ItemType.Flippers);
                    CancelAim();
                    return;
                }
            }
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            CancelAim();
            return;
        }

        if (aiming == ItemType.Ball)
        {
            if (turnManager.TryThrowBall(cell))
            {
                Consume(ItemType.Ball);
                CancelAim();
                return;
            }
            SoundManager.Instance.PlaySfx(SfxType.Deny);
            CancelAim();
        }
    }

    public void CancelAim()
    {
        aiming = null;
        if (itemBar != null) itemBar.SetActiveItem(null);
        ClearMarkers();
    }

    private void Consume(ItemType type)
    {
        ConsumeOne(type);
        PushCounts();
    }

    private void PushCounts()
    {
        itemBar.SetCounts(GetCount(ItemType.Flippers), GetCount(ItemType.Ball), GetCount(ItemType.Shield));
    }

    private void ShowDashMarkers()
    {
        foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int via = player.GridPosition + dir;
            Vector2Int target = player.GridPosition + dir * 2;
            if (!gridManager.IsWalkable(via) || !gridManager.IsWalkable(target)) continue;
            CreateMarker(target, new Color(0.2f, 0.85f, 0.95f, 0.5f), 0.5f);
        }
    }

    private void ShowBallMarkers()
    {
        Vector2Int p = player.GridPosition;
        for (int dx = -BallThrowRange; dx <= BallThrowRange; dx++)
        {
            for (int dy = -BallThrowRange; dy <= BallThrowRange; dy++)
            {
                if (Mathf.Abs(dx) + Mathf.Abs(dy) > BallThrowRange) continue;
                if (dx == 0 && dy == 0) continue;
                Vector2Int cell = p + new Vector2Int(dx, dy);
                if (!gridManager.IsWalkable(cell)) continue;
                CreateMarker(cell, new Color(0.96f, 0.65f, 0.14f, 0.35f), 0.34f);
            }
        }
    }

    private void CreateMarker(Vector2Int cell, Color color, float scale)
    {
        var go = new GameObject("AimMarker");
        go.transform.SetParent(transform, false);
        go.transform.position = gridManager.GridToWorld(cell);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Circle;
        sr.color = color;
        sr.sortingOrder = 5;
        go.transform.localScale = Vector3.one * scale;
        go.transform.DOScale(scale * 1.15f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        aimMarkers.Add(go);
    }

    private void ClearMarkers()
    {
        foreach (var marker in aimMarkers)
        {
            if (marker != null)
            {
                marker.transform.DOKill();
                Destroy(marker);
            }
        }
        aimMarkers.Clear();
    }
}
