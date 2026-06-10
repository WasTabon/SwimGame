using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoveHighlighter : MonoBehaviour
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    private GridManager grid;
    private readonly List<GameObject> markers = new List<GameObject>();

    public void Init(GridManager gridManager)
    {
        grid = gridManager;
        foreach (var marker in markers)
        {
            if (marker != null) Destroy(marker);
        }
        markers.Clear();

        for (int i = 0; i < Directions.Length; i++)
        {
            var go = new GameObject("MoveMarker_" + i);
            go.transform.SetParent(transform, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.Circle;
            sr.color = new Color(1f, 1f, 1f, 0.35f);
            sr.sortingOrder = 5;
            go.transform.localScale = Vector3.one * 0.4f;
            go.transform.DOScale(0.48f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            go.SetActive(false);
            markers.Add(go);
        }
    }

    public void Refresh(Vector2Int playerPos)
    {
        for (int i = 0; i < Directions.Length; i++)
        {
            Vector2Int cell = playerPos + Directions[i];
            bool walkable = grid.IsWalkable(cell);
            markers[i].SetActive(walkable);
            if (walkable)
            {
                markers[i].transform.position = grid.GridToWorld(cell);
            }
        }
    }

    public void Hide()
    {
        foreach (var marker in markers)
        {
            marker.SetActive(false);
        }
    }
}
