using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CellType { Water, Wall, Exit }

public enum SwimmerType { Linear, Patrol, Fast, Delayed, Reactive }

public class SwimmerSpawn
{
    public SwimmerType type;
    public Vector2Int position;
    public Vector2Int direction;
}

public class ParsedLevel
{
    public int width;
    public int height;
    public CellType[,] cells;
    public Vector2Int playerStart;
    public Vector2Int exitPosition;
    public List<SwimmerSpawn> swimmers = new List<SwimmerSpawn>();
    public List<List<Vector2Int>> patrols = new List<List<Vector2Int>>();
}

[CreateAssetMenu(fileName = "Level", menuName = "SwimGame/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName = "Level";
    [TextArea(5, 30)] public string layout;
    public List<string> patrolRoutes = new List<string>();

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

        void AddSwimmer(SwimmerType type, int x, int y, Vector2Int dir)
        {
            result.swimmers.Add(new SwimmerSpawn
            {
                type = type,
                position = new Vector2Int(x, y),
                direction = dir
            });
        }

        for (int row = 0; row < height; row++)
        {
            string line = lines[row];
            int y = height - 1 - row;
            for (int x = 0; x < width; x++)
            {
                char c = x < line.Length ? line[x] : '#';
                bool isWall = false;
                switch (c)
                {
                    case '#':
                        isWall = true;
                        break;
                    case 'E':
                        result.cells[x, y] = CellType.Exit;
                        result.exitPosition = new Vector2Int(x, y);
                        exitFound = true;
                        break;
                    case 'P':
                        result.playerStart = new Vector2Int(x, y);
                        playerFound = true;
                        break;
                    case '^': AddSwimmer(SwimmerType.Linear, x, y, Vector2Int.up); break;
                    case 'v': AddSwimmer(SwimmerType.Linear, x, y, Vector2Int.down); break;
                    case '<': AddSwimmer(SwimmerType.Linear, x, y, Vector2Int.left); break;
                    case '>': AddSwimmer(SwimmerType.Linear, x, y, Vector2Int.right); break;
                    case 'U': AddSwimmer(SwimmerType.Fast, x, y, Vector2Int.up); break;
                    case 'D': AddSwimmer(SwimmerType.Fast, x, y, Vector2Int.down); break;
                    case 'L': AddSwimmer(SwimmerType.Fast, x, y, Vector2Int.left); break;
                    case 'R': AddSwimmer(SwimmerType.Fast, x, y, Vector2Int.right); break;
                    case 'u': AddSwimmer(SwimmerType.Delayed, x, y, Vector2Int.up); break;
                    case 'd': AddSwimmer(SwimmerType.Delayed, x, y, Vector2Int.down); break;
                    case 'l': AddSwimmer(SwimmerType.Delayed, x, y, Vector2Int.left); break;
                    case 'r': AddSwimmer(SwimmerType.Delayed, x, y, Vector2Int.right); break;
                    case '8': AddSwimmer(SwimmerType.Reactive, x, y, Vector2Int.up); break;
                    case '2': AddSwimmer(SwimmerType.Reactive, x, y, Vector2Int.down); break;
                    case '4': AddSwimmer(SwimmerType.Reactive, x, y, Vector2Int.left); break;
                    case '6': AddSwimmer(SwimmerType.Reactive, x, y, Vector2Int.right); break;
                }
                if (c != 'E')
                {
                    result.cells[x, y] = isWall ? CellType.Wall : CellType.Water;
                }
            }
        }

        if (patrolRoutes != null)
        {
            foreach (var routeStr in patrolRoutes)
            {
                if (string.IsNullOrWhiteSpace(routeStr)) continue;
                var points = new List<Vector2Int>();
                foreach (var token in routeStr.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries))
                {
                    var parts = token.Split(',');
                    if (parts.Length != 2) continue;
                    points.Add(new Vector2Int(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim())));
                }
                if (points.Count >= 2) result.patrols.Add(points);
            }
        }

        Debug.Assert(playerFound, "Level '" + levelName + "' has no player start (P)!");
        Debug.Assert(exitFound, "Level '" + levelName + "' has no exit (E)!");
        return result;
    }
}
