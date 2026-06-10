using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CellType { Water, Wall, Exit }

public class ParsedLevel
{
    public int width;
    public int height;
    public CellType[,] cells;
    public Vector2Int playerStart;
    public Vector2Int exitPosition;
}

[CreateAssetMenu(fileName = "Level", menuName = "SwimGame/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName = "Level";
    [TextArea(5, 30)] public string layout;

    public ParsedLevel Parse()
    {
        List<string> lines = layout.Replace("\r", "").Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToList();

        int height = lines.Count;
        int width = lines.Max(l => l.Length);
        var result = new ParsedLevel
        {
            width = width,
            height = height,
            cells = new CellType[width, height]
        };

        bool playerFound = false;
        bool exitFound = false;

        for (int row = 0; row < height; row++)
        {
            string line = lines[row];
            int y = height - 1 - row;
            for (int x = 0; x < width; x++)
            {
                char c = x < line.Length ? line[x] : '#';
                switch (c)
                {
                    case '#':
                        result.cells[x, y] = CellType.Wall;
                        break;
                    case 'E':
                        result.cells[x, y] = CellType.Exit;
                        result.exitPosition = new Vector2Int(x, y);
                        exitFound = true;
                        break;
                    case 'P':
                        result.cells[x, y] = CellType.Water;
                        result.playerStart = new Vector2Int(x, y);
                        playerFound = true;
                        break;
                    default:
                        result.cells[x, y] = CellType.Water;
                        break;
                }
            }
        }

        Debug.Assert(playerFound, "Level '" + levelName + "' has no player start (P)!");
        Debug.Assert(exitFound, "Level '" + levelName + "' has no exit (E)!");
        return result;
    }
}
