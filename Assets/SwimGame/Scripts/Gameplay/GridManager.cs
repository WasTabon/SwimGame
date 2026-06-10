using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const float CellSize = 1f;

    private static readonly Color WaterColorA = new Color32(74, 144, 226, 255);
    private static readonly Color WaterColorB = new Color32(66, 132, 210, 255);
    private static readonly Color WallColor = new Color32(52, 73, 94, 255);
    private static readonly Color ExitColor = new Color32(46, 204, 113, 255);
    private static readonly Color ExitColorBright = new Color32(96, 235, 155, 255);
    private static readonly Color CurrentTint = new Color(0.78f, 0.82f, 0.95f, 1f);

    [SerializeField] private Transform cellsContainer;

    private ParsedLevel level;
    private readonly Dictionary<Vector2Int, SwimmerBase> occupancy = new Dictionary<Vector2Int, SwimmerBase>();
    private readonly HashSet<Vector2Int> boatCells = new HashSet<Vector2Int>();
    private readonly HashSet<Vector2Int> blockedPlatformCells = new HashSet<Vector2Int>();

    public int Width => level.width;
    public int Height => level.height;
    public Vector2Int ExitPosition => level.exitPosition;
    public Vector2Int PlayerStart => level.playerStart;

    public void Build(ParsedLevel parsedLevel)
    {
        level = parsedLevel;
        occupancy.Clear();
        boatCells.Clear();
        blockedPlatformCells.Clear();

        for (int i = cellsContainer.childCount - 1; i >= 0; i--)
        {
            var child = cellsContainer.GetChild(i);
            child.DOKill();
            var childSr = child.GetComponent<SpriteRenderer>();
            if (childSr != null) childSr.DOKill();
            Destroy(child.gameObject);
        }

        for (int x = 0; x < level.width; x++)
        {
            for (int y = 0; y < level.height; y++)
            {
                var cell = new Vector2Int(x, y);
                var go = new GameObject("Cell_" + x + "_" + y);
                go.transform.SetParent(cellsContainer, false);
                go.transform.position = GridToWorld(cell);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteFactory.Square;
                sr.sortingOrder = 0;

                switch (level.cells[x, y])
                {
                    case CellType.Wall:
                        sr.color = WallColor;
                        break;
                    case CellType.Exit:
                        sr.color = ExitColor;
                        sr.DOColor(ExitColorBright, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                        go.transform.DOScale(1.06f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                        break;
                    default:
                        Color water = (x + y) % 2 == 0 ? WaterColorA : WaterColorB;
                        if (level.currents.ContainsKey(cell)) water *= CurrentTint;
                        water.a = 1f;
                        sr.color = water;
                        break;
                }
            }
        }

        foreach (var kvp in level.currents)
        {
            if (!IsInside(kvp.Key)) continue;
            if (level.cells[kvp.Key.x, kvp.Key.y] == CellType.Wall) continue;
            BuildCurrentArrow(kvp.Key, kvp.Value);
        }
    }

    private void BuildCurrentArrow(Vector2Int cell, Vector2Int dir)
    {
        var go = new GameObject("Current_" + cell.x + "_" + cell.y);
        go.transform.SetParent(cellsContainer, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Triangle;
        sr.color = new Color(1f, 1f, 1f, 0.4f);
        sr.sortingOrder = 1;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        go.transform.localScale = Vector3.one * 0.3f;

        Vector3 center = GridToWorld(cell);
        Vector3 dirV = new Vector3(dir.x, dir.y, 0f);
        go.transform.position = center - dirV * 0.16f;
        go.transform.DOMove(center + dirV * 0.16f, 0.9f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public Vector2Int GetCurrent(Vector2Int cell)
    {
        level.currents.TryGetValue(cell, out var dir);
        return dir;
    }

    public void AddBoatCells(List<Vector2Int> cells)
    {
        foreach (var cell in cells) boatCells.Add(cell);
    }

    public void RemoveBoatCells(List<Vector2Int> cells)
    {
        foreach (var cell in cells) boatCells.Remove(cell);
    }

    public void SetPlatformBlocked(Vector2Int cell, bool blocked)
    {
        if (blocked) blockedPlatformCells.Add(cell);
        else blockedPlatformCells.Remove(cell);
    }

    public void ClearSwimmers()
    {
        occupancy.Clear();
    }

    public void RegisterSwimmer(SwimmerBase swimmer, Vector2Int cell)
    {
        occupancy[cell] = swimmer;
    }

    public void MoveSwimmer(SwimmerBase swimmer, Vector2Int from, Vector2Int to)
    {
        if (occupancy.TryGetValue(from, out var existing) && existing == swimmer)
        {
            occupancy.Remove(from);
        }
        occupancy[to] = swimmer;
    }

    public SwimmerBase GetSwimmerAt(Vector2Int cell)
    {
        occupancy.TryGetValue(cell, out var swimmer);
        return swimmer;
    }

    public Vector3 GridToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x * CellSize, cell.y * CellSize, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        return new Vector2Int(Mathf.RoundToInt(world.x / CellSize), Mathf.RoundToInt(world.y / CellSize));
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < level.width && cell.y >= 0 && cell.y < level.height;
    }

    public bool IsWalkable(Vector2Int cell)
    {
        return IsInside(cell)
               && level.cells[cell.x, cell.y] != CellType.Wall
               && !boatCells.Contains(cell)
               && !blockedPlatformCells.Contains(cell);
    }

    public CellType GetCell(Vector2Int cell)
    {
        return level.cells[cell.x, cell.y];
    }
}
