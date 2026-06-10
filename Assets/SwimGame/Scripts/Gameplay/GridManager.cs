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

    [SerializeField] private Transform cellsContainer;

    private ParsedLevel level;
    private readonly Dictionary<Vector2Int, SwimmerBase> occupancy = new Dictionary<Vector2Int, SwimmerBase>();

    public int Width => level.width;
    public int Height => level.height;
    public Vector2Int ExitPosition => level.exitPosition;
    public Vector2Int PlayerStart => level.playerStart;

    public void Build(ParsedLevel parsedLevel)
    {
        level = parsedLevel;
        occupancy.Clear();

        for (int i = cellsContainer.childCount - 1; i >= 0; i--)
        {
            var child = cellsContainer.GetChild(i);
            child.DOKill();
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null) sr.DOKill();
            Destroy(child.gameObject);
        }

        for (int x = 0; x < level.width; x++)
        {
            for (int y = 0; y < level.height; y++)
            {
                var go = new GameObject("Cell_" + x + "_" + y);
                go.transform.SetParent(cellsContainer, false);
                go.transform.position = GridToWorld(new Vector2Int(x, y));
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
                        sr.color = (x + y) % 2 == 0 ? WaterColorA : WaterColorB;
                        break;
                }
            }
        }
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
        return IsInside(cell) && level.cells[cell.x, cell.y] != CellType.Wall;
    }

    public CellType GetCell(Vector2Int cell)
    {
        return level.cells[cell.x, cell.y];
    }
}
