using System.Collections.Generic;
using UnityEngine;

public class PipeGridManager : MonoBehaviour
{
    public static PipeGridManager instance;

    public int rows;
    public int cols;

    public PipeTile[] allTiles;
    public PipeTile sourceTile;
    public PipeTile[] homeTiles;

    PipeTile[,] grid;

    // 0 Up (+Z), 1 Right (+X), 2 Down, 3 Left
    static readonly Vector2Int[] Dir =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
    };
    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void BuildGrid()
    {
        grid = new PipeTile[rows, cols];

        foreach (var t in allTiles)
        {
            if (!InBounds(new Vector2Int(t.gridX, t.gridY)))
            {
                Debug.LogError($"Tile out of bounds: {t.name}");
                continue;
            }

            if (grid[t.gridX, t.gridY] != null)
            {
                Debug.LogError($"Duplicate tile at {t.gridX},{t.gridY}");
                continue;
            }

            grid[t.gridX, t.gridY] = t;
        }
    }

    HashSet<Vector2Int> visited = new();
    Queue<Vector2Int> queue = new();

    public void CheckConnection(bool isFirstCheck = false)
    {
        visited.Clear();
        queue.Clear();

        if (sourceTile == null) return;

        Vector2Int start = new Vector2Int(sourceTile.gridX, sourceTile.gridY);
        visited.Add(start);
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Vector2Int curPos = queue.Dequeue();
            PipeTile curTile = grid[curPos.x, curPos.y];
            if (curTile == null || curTile.pipeType == PipeType.None) continue;

            HashSet<int> curPorts = curTile.GetOpenPorts();

            foreach (int port in curPorts)
            {
                Vector2Int nextPos = curPos + Dir[port];

                if (!InBounds(nextPos)) continue;
                if (visited.Contains(nextPos)) continue;

                PipeTile nextTile = grid[nextPos.x, nextPos.y];
                if (nextTile == null || nextTile.pipeType == PipeType.None) continue;

                int opposite = (port + 2) % 4;
                if (!nextTile.GetOpenPorts().Contains(opposite)) continue;

                visited.Add(nextPos);
                queue.Enqueue(nextPos);
            }
        }

        foreach (PipeTile t in allTiles)
        {
            Vector2Int pos = new Vector2Int(t.gridX, t.gridY);
            bool shouldConnect = visited.Contains(pos);
            t.SetConnected(shouldConnect, !isFirstCheck);
        }

        bool solved = true;
        foreach (PipeTile home in homeTiles)
        {
            Vector2Int pos = new Vector2Int(home.gridX, home.gridY);
            if (!visited.Contains(pos))
            {
                solved = false;
                break;
            }
        }

        if (solved)
            GameManager.instance.GameStateCallBack(GameState.Win);
    }

    bool InBounds(Vector2Int p) => p.x >= 0 && p.x < rows && p.y >= 0 && p.y < cols;

}