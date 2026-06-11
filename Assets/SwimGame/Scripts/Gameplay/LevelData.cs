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

public class BoatSpawn
{
    public Vector2Int anchor;
    public int length;
    public Vector2Int direction;
}

public class PlatformSpawn
{
    public Vector2Int cell;
    public int openTurns;
    public int closedTurns;
    public int offset;
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
    public Dictionary<Vector2Int, Vector2Int> currents = new Dictionary<Vector2Int, Vector2Int>();
    public List<BoatSpawn> boats = new List<BoatSpawn>();
    public List<PlatformSpawn> platforms = new List<PlatformSpawn>();
}

[CreateAssetMenu(fileName = "Level", menuName = "SwimGame/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName = "Level";
    public int optimalMoves = 10;
    [TextArea(5, 30)] public string layout;
    public List<string> patrolRoutes = new List<string>();
    public List<string> currentZones = new List<string>();
    public List<string> boats = new List<string>();
    public List<string> platforms = new List<string>();

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
                foreach (var token in Tokens(routeStr))
                {
                    points.Add(ParsePoint(token));
                }
                if (points.Count >= 2) result.patrols.Add(points);
            }
        }

        if (currentZones != null)
        {
            foreach (var zoneStr in currentZones)
            {
                if (string.IsNullOrWhiteSpace(zoneStr)) continue;
                var tokens = Tokens(zoneStr);
                if (tokens.Length < 3) continue;
                Vector2Int a = ParsePoint(tokens[0]);
                Vector2Int b = ParsePoint(tokens[1]);
                Vector2Int dir = ParseDirection(tokens[2]);
                if (dir == Vector2Int.zero) continue;
                for (int x = Mathf.Min(a.x, b.x); x <= Mathf.Max(a.x, b.x); x++)
                {
                    for (int y = Mathf.Min(a.y, b.y); y <= Mathf.Max(a.y, b.y); y++)
                    {
                        result.currents[new Vector2Int(x, y)] = dir;
                    }
                }
            }
        }

        if (boats != null)
        {
            foreach (var boatStr in boats)
            {
                if (string.IsNullOrWhiteSpace(boatStr)) continue;
                var tokens = Tokens(boatStr);
                if (tokens.Length < 3) continue;
                result.boats.Add(new BoatSpawn
                {
                    anchor = ParsePoint(tokens[0]),
                    length = int.Parse(tokens[1]),
                    direction = ParseDirection(tokens[2])
                });
            }
        }

        if (platforms != null)
        {
            foreach (var platStr in platforms)
            {
                if (string.IsNullOrWhiteSpace(platStr)) continue;
                var tokens = Tokens(platStr);
                if (tokens.Length < 4) continue;
                result.platforms.Add(new PlatformSpawn
                {
                    cell = ParsePoint(tokens[0]),
                    openTurns = int.Parse(tokens[1]),
                    closedTurns = int.Parse(tokens[2]),
                    offset = int.Parse(tokens[3])
                });
            }
        }

        Debug.Assert(playerFound, "Level '" + levelName + "' has no player start (P)!");
        Debug.Assert(exitFound, "Level '" + levelName + "' has no exit (E)!");
        return result;
    }

    private static string[] Tokens(string s)
    {
        return s.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    private static Vector2Int ParsePoint(string token)
    {
        var parts = token.Split(',');
        return new Vector2Int(int.Parse(parts[0].Trim()), int.Parse(parts[1].Trim()));
    }

    private static Vector2Int ParseDirection(string word)
    {
        switch (word.ToLowerInvariant())
        {
            case "up": return Vector2Int.up;
            case "down": return Vector2Int.down;
            case "left": return Vector2Int.left;
            case "right": return Vector2Int.right;
            default: return Vector2Int.zero;
        }
    }
}
