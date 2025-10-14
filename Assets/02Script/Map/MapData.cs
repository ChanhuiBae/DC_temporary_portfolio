using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down,
}
public enum TileType
{
    None = -1,
    Secret = 0,
    End = 1,
    Two = 2,
    Tree = 3,
    Four = 4,
    Exit = 5,
    Boss = 6,
    MiddleBoss = 7,
    Monster = 8,
    Chest = 9,
    PositiveEvent = 10,
    NegativeEvent = 11,
    Merchant = 12,
}

public class MapData
{
    public static int[] sizes = { 7, 7, 9, 9, 9, 11, 11, 11 };
    public static int[] maxs = { 16, 16, 23, 27, 35, 45, 55, 70 };
    public static int[] mins = { 10, 10, 15, 19, 25, 35, 45, 60 };

    public int floor;
    public int size;
    public int max;
    public int min;

    public int[,] map;
    public bool[,] findMap;
    public bool[,] knownMap;
    public int[,] goneMap;
    public bool[,] blownupMap;
    public bool[,] searchMap;

    public Vector2Int position;
    public Direction direction;

    public Vector2Int startPosition;


    public bool SetSize(int floor)
    {
        if(floor > -1 && floor < 8)
        {
            this.floor = floor;
            this.size = sizes[floor];
            this.max = maxs[floor];
            this.min = mins[floor];
            return true;
        }
        return false;
    }
    public bool Copy(string[] m, string[] find, string[] know, string[] gone, string[] blown, string[] search)
    {
        map = new int[size, size];
        findMap = new bool[size, size];
        knownMap = new bool[size, size];
        goneMap = new int[size, size];
        blownupMap = new bool[size, size];
        searchMap = new bool[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = Int32.Parse(m[i * size + j]);
                if (find[i * size + j] == "1") findMap[i, j] = true; else findMap[i, j] = false;
                if (know[i * size + j] == "1") knownMap[i, j] = true; else knownMap[i, j] = false;
                goneMap[i, j] = Int32.Parse(gone[i * size + j]);
                if (blown[i * size + j] == "1") blownupMap[i, j] = true; else blownupMap[i, j] = false;
                if (search[i * size + j] == "1") searchMap[i, j] = true; else searchMap[i, j] = false;
            }
        }
        return true;
    }
    public void Copy(MapData data)
    {
        map = new int[size, size];
        findMap = new bool[size, size];
        knownMap = new bool[size, size];
        goneMap = new int[size, size];
        blownupMap = new bool[size, size];
        searchMap = new bool[size, size];

        position = data.position;
        direction = data.direction;
        startPosition = data.startPosition;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                map[i, j] = data.map[i, j];
                findMap[i, j] = data.findMap[i, j];
                knownMap[i, j] = data.knownMap[i, j];
                goneMap[i, j] = data.goneMap[i, j];
                blownupMap[i, j] = data.blownupMap[i, j];
                searchMap[i, j] = data.searchMap[i, j];
            }
        }
    }

    public int minDistance(Vector2Int start, Vector2Int target)
    { // Astar
        int result = -1; // distance
        int failResult = -1;
        int obstacle = 0;
        List<List<MapTile>> tiles = new List<List<MapTile>>();
        List<MapTile> openList = new List<MapTile>();
        List<MapTile> closeList = new List<MapTile>();
        List<MapTile> tile = new List<MapTile>();
        MapTile startTile = null;
        MapTile targetTile = null;
        // Initial values
        for (int i = 0; i < size; i++)
        {
            List<MapTile> t = new List<MapTile>();
            for (int j = 0; j < size; j++)
            {
                MapTile temp = new MapTile();
                temp.X = i;
                temp.Y = j;
                t.Add(temp);
                if (i == target.x && j == target.y)
                {
                    targetTile = temp;
                }
            }
            tiles.Add(t);
        }

        startTile = tiles[start.x][start.y];
        openList.Add(startTile);
        if (targetTile == null)
        {
            // can not found target
            return failResult;
        }
        MapTile currentTile = null;
        do
        {
            if (openList.Count == 0)
            {
                break;
            }
            currentTile = openList.OrderBy(o => o.F).First();
            openList.Remove(currentTile);
            closeList.Add(currentTile);
            if (currentTile == targetTile)
            {
                break;
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //// 8 way
                    //bool near = (System.Math.Abs(currentPath.X - pathes[i][j].X) <= 1)
                    //         && (System.Math.Abs(currentPath.Y - pathes[i][j].Y) <= 1);
                    // 4 way
                    bool near = (System.Math.Abs(currentTile.X - tiles[i][j].X) <= 1)
                             && (System.Math.Abs(currentTile.Y - tiles[i][j].Y) <= 1)
                             && (currentTile.Y == tiles[i][j].Y || currentTile.X == tiles[i][j].X);
                    if (map[i, j] == obstacle
                     || closeList.Contains(tiles[i][j])
                     || (!near))
                    {
                        continue;
                    }
                    if (!openList.Contains(tiles[i][j]))
                    {
                        openList.Add(tiles[i][j]);
                        tiles[i][j].Execute(currentTile, targetTile);
                    }
                    else
                    {
                        if (MapTile.CalcGValue(currentTile, tiles[i][j]) < tiles[i][j].G)
                        {
                            tiles[i][j].Execute(currentTile, targetTile);
                        }
                    }
                }
            }
        } while (currentTile != null);
        if (currentTile != targetTile)
        {
            // can not found root
            return failResult;
        }
        do
        {
            tile.Add(currentTile);
            currentTile = currentTile.Parent;
        }
        while (currentTile != null);
        tile.Reverse();
        result = tile.Count - 1;
        return result;
    }

    public Direction GetDirection(Vector3 point)
    {
        if (point.y >= position.y * 12 + 3 && point.y <= position.y * 12 + 10)
        {
            if (point.x >= (position.x + 1) * 12 + 2 && point.x <= (position.x + 1) * 12 + 9)
            {
                return Direction.Right;
            }
            if (point.x >= (position.x - 1) * 12 + 2 && point.x <= (position.x - 1) * 12 + 9)
            {
                return Direction.Left;
            }
        }
        if (point.x >= position.x * 12 + 2 && point.x <= position.x * 12 + 9)
        {
            if (point.y >= (position.y + 1) * 12 + 3 && point.y <= (position.y + 1) * 12 + 10)
            {
                return Direction.Up;
            }
            if (point.y >= (position.y - 1) * 12 + 3 && point.y <= (position.y - 1) * 12 + 10)
            {
                return Direction.Down;
            }
        }

        return Direction.None;
    }

    public void UpdateFindMap(Vector2Int point)
    {
        findMap[point.x, point.y] = true;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] != (int)TileType.Secret)
        {
            findMap[point.x - 1, point.y] = true;
        }
        if (point.x + 1 < size && map[point.x + 1, point.y] != (int)TileType.Secret)
        {
            findMap[point.x + 1, point.y] = true;
        }
        if (point.y + 1 < size && map[point.x, point.y + 1] != (int)TileType.Secret)
        {
            findMap[point.x, point.y + 1] = true;
        }
        if (point.y - 1 > -1 && map[point.x, point.y - 1] != (int)TileType.Secret)
        {
            findMap[point.x, point.y - 1] = true;
        }
    }

    
    public bool Check3Connect2(Vector2Int point)
    {
        int connect = 0;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < size && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < size && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect > 2)
        {
            if (point.x - 1 > -1 && map[point.x - 1, point.y] > 1)
                return false;
            if (point.x + 1 < size && map[point.x + 1, point.y] > 1)
                return false;
            if (point.y + 1 < size && map[point.x, point.y + 1] > 1)
                return false;
            if (point.y - 1 > -1 && map[point.x, point.y - 1] > 1)
                return false;
        }
        return true;
    }

    public bool CheckDerived3Connect2(Vector2Int point)
    {
        if (map[point.x, point.y] == -1)
            return true;

        if (map[point.x, point.y] > 1)
        {
            if (point.x - 1 > -1 && map[point.x - 1, point.y] > 2)
                return false;
            if (point.x + 1 < size && map[point.x + 1, point.y] > 2)
                return false;
            if (point.y + 1 < size && map[point.x, point.y + 1] > 2)
                return false;
            if (point.y - 1 > -1 && map[point.x, point.y - 1] > 2)
                return false;
        }
        return true;
    }

    public bool CheckConnect1(Vector2Int point)
    {
        bool checkPoint = Check3Connect2(point);
        bool checkLeft = true;
        bool checkRight = true;
        bool checkUp = true;
        bool checkDown = true;
        if (point.x - 1 > -1)
        {
            checkLeft = CheckDerived3Connect2(new Vector2Int(point.x - 1, point.y));
        }
        if (point.x + 1 < size)
        {
            checkRight = CheckDerived3Connect2(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < size)
        {
            checkUp = CheckDerived3Connect2(new Vector2Int(point.x, point.y + 1));
        }
        if (point.y - 1 > -1)
        {
            checkDown = CheckDerived3Connect2(new Vector2Int(point.x, point.y - 1));
        }
        return checkPoint && checkLeft && checkRight && checkUp && checkDown;
    }
    private bool Check4Connect44(Vector2Int point)
    {
        bool leftOut = point.x - 1 > -1;
        bool rightOut = point.x + 1 < size;
        bool upOut = point.y + 1 < size;
        bool downOut = point.y - 1 > -1;

        int connect = 0;

        if (point.x - 1 > -1 && map[point.x - 1, point.y] > -1)
            connect++;
        if (point.x + 1 < size && map[point.x + 1, point.y] > -1)
            connect++;
        if (point.y + 1 < size && map[point.x, point.y + 1] > -1)
            connect++;
        if (point.y - 1 > -1 && map[point.x, point.y - 1] > -1)
            connect++;

        if (connect == 4)
        {
            int around = 1;
            if (leftOut && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (rightOut && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (point.x + 2 < size && map[point.x + 2, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (upOut && map[point.x, point.y + 1] > 2)
            {
                around++;
            }
            if (point.y + 2 < size && map[point.x, point.y + 2] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (downOut && map[point.x, point.y - 1] > 2)
            {
                around++;
            }
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (leftOut && map[point.x - 1, point.y] > 2)
            {
                around++;
            }
            if (rightOut && map[point.x + 1, point.y] > 2)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (upOut && map[point.x, point.y + 1] > 2)
            {
                around++;
            }
            if (downOut && map[point.x, point.y - 1] > 2)
            {
                around++;
            }
            if (around == 3)
                return false;
        }
        return true;
    }
    public bool CheckDerived4Connect44(Vector2Int point)
    {
        if (map[point.x, point.y] == -1)
            return true;

        if (map[point.x, point.y] > 2)
        {
            int around = 1;
            if (point.x - 2 > -1 && map[point.x - 2, point.y] > 3)
            {
                around++;
            }
            if (point.x - 3 > -1 && map[point.x - 3, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.x + 2 < size && map[point.x + 2, point.y] > 3)
            {
                around++;
            }
            if (point.x + 3 < size && map[point.x + 3, point.y] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.y + 2 < size && map[point.x, point.y + 2] > 3)
            {
                around++;
            }
            if (point.y + 3 < size && map[point.x, point.y + 3] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
            around = 1;
            if (point.y - 2 > -1 && map[point.x, point.y - 2] > 3)
            {
                around++;
            }
            if (point.y - 3 > -1 && map[point.x, point.y - 3] > 3)
            {
                around++;
            }
            if (around == 3)
                return false;
        }
        return true;
    }

    public bool CheckConnect2(Vector2Int point)
    {
        bool checkPoint = Check4Connect44(point);
        bool checkLeft = true;
        bool checkRight = true;
        bool checkUp = true;
        bool checkDown = true;
        if (point.x - 1 > -1)
        {
            checkLeft = CheckDerived4Connect44(new Vector2Int(point.x - 1, point.y));
        }
        if (point.x + 1 < size)
        {
            checkRight = CheckDerived4Connect44(new Vector2Int(point.x + 1, point.y));
        }
        if (point.y + 1 < size)
        {
            checkUp = CheckDerived4Connect44(new Vector2Int(point.x, point.y + 1));
        }
        if (point.y - 1 > -1)
        {
            checkDown = CheckDerived4Connect44(new Vector2Int(point.x, point.y - 1));
        }
        return checkPoint && checkLeft && checkRight && checkUp && checkDown;
    }
}