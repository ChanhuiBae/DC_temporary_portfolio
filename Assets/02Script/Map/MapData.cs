using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction
{
    None,
    Left,
    Right,
    Up,
    Down,
}

public class MapData
{
    public int floor;
    public int size;
    public int max;
    public int min;
    public int[,] map;
    public bool[,] findMap;
    public int[,] turn;
    public bool[,] knownMap;
    public int[,] goneMap;
    public bool[,] blownupMap;
    public bool[,] searchMap;

    public Vector2Int position;
    public Direction direction;

    public Vector2Int startPosition;


    public bool SetSize(int floor)
    {
        if(floor > 0 && floor < 7)
        {
            this.floor = floor;
            size = TileData.size[floor];
            max = TileData.max[floor];
            min = TileData.min[floor];
            return true;
        }
        return false;
    }

    public bool Copy(string[] m, string[] find, string[] know, string[] gone, string[] blown, string[] search)
    {
        map = new int[size, size];
        findMap = new bool[size, size];
        turn = new int[size, size];
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

    public int GetMinDistance(Vector2Int start, Vector2Int target)
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


    public int CountAdjacent4Tile(Vector2Int point)
    {
        int count = 0;
        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < size;
        bool up = point.y + 1 < size;
        bool down = point.y - 1 > -1;

        if (up && map[point.x, point.y + 1] > 0)
            count++;
        if (down && map[point.x, point.y - 1] > 0)
            count++; 
        if (left && map[point.x - 1, point.y] > 0)
            count++;
        if (right && map[point.x + 1, point.y] > 0)
            count++;
        return count;
    }
    public int CountDiagonalTile(Vector2Int point)
    {
        int count = 0;
        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < size;
        bool up = point.y + 1 < size;
        bool down = point.y - 1 > -1;

        if (left && up && map[point.x - 1, point.y + 1] > 0)
            count++;
        if (right && down && map[point.x + 1, point.y - 1] > 0)
            count++;
        if (up && right && map[point.x + 1, point.y + 1] > 0)
            count++;
        if (down && left && map[point.x - 1, point.y - 1] > 0)
            count++;
        return count;
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

    public bool CheckContinue3(Vector2Int point, int type, int count, int cut)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < size;
        bool upIn = point.y + 1 < size;
        bool downIn = point.y - 1 > -1;
        int reset = count;
        if (leftIn)
        {
            if(map[point.x - 1, point.y] != type)
            {
                if (upIn && map[point.x - 1, point.y + 1] == type)
                    count++;
                if (downIn && map[point.x - 1, point.y - 1] == type)
                    count++;
            }
            else
            {
                count++;
                if (upIn && map[point.x - 1, point.y + 1] == type)
                    count++;
                if (downIn && map[point.x - 1, point.y - 1] == type)
                    count++;
            }
            if (count > cut)
                return false;
        }

        if (rightIn)
        {
            count = reset;
            if(map[point.x + 1, point.y] != type)
            {
                if (upIn && map[point.x + 1, point.y + 1] == type)
                    count++;
                if (downIn && map[point.x + 1, point.y - 1] == type)
                    count++;
            }
            else
            {
                count++;
                if (upIn && map[point.x + 1, point.y + 1] == type)
                    count++;
                if (downIn && map[point.x + 1, point.y - 1] == type)
                    count++;
            }
            if (count > cut)
                return false;
        }
        if (upIn)
        {
            count = reset;
            if(map[point.x, point.y + 1] != type)
            {
                if (leftIn && map[point.x - 1, point.y + 1] == type)
                    count++;
                if (rightIn && map[point.x + 1, point.y + 1] == type)
                    count++;
            }
            else
            {
                count++;
                if (leftIn && map[point.x - 1, point.y + 1] == type)
                    count++;
                if (rightIn && map[point.x + 1, point.y + 1] == type)
                    count++;
            }
            if (count > cut)
                return false;
        }
        if (downIn)
        {
            count = reset;
            if(map[point.x, point.y - 1] != type)
            {
                if (leftIn && map[point.x - 1, point.y - 1] == type)
                    count++;
                if (rightIn && map[point.x + 1, point.y - 1] == type)
                    count++;
            }
            else
            {
                count++;
                if (leftIn && map[point.x - 1, point.y - 1] == type)
                    count++;
                if (rightIn && map[point.x + 1, point.y - 1] == type)
                    count++;
            }
            if (count > cut)
                return false;
        }

        return true;
    }

    public bool CheckContinue4(Vector2Int point, int type, int cut)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < size;
        bool upIn = point.y + 1 < size;
        bool downIn = point.y - 1 > -1;

        if (leftIn)
        {
            if(map[point.x - 1, point.y] != type) 
                return CheckContinue3(new Vector2Int(point.x - 1, point.y), type, 0, cut);
            else
                return CheckContinue3(new Vector2Int(point.x - 1, point.y), type, 1, cut);
        }
            
        if (rightIn)
        {
            if(map[point.x + 1, point.y] != type) 
                return CheckContinue3(new Vector2Int(point.x + 1, point.y), type, 0, cut);
            else
                return CheckContinue3(new Vector2Int(point.x + 1, point.y), type, 1, cut);
        }
            
        if (upIn)
        {
            if (map[point.x, point.y + 1] != type)
                return CheckContinue3(new Vector2Int(point.x, point.y + 1), type, 0, cut);
            else
                return CheckContinue3(new Vector2Int(point.x, point.y + 1), type, 1, cut);
        }

        if (downIn)
        {
            if(map[point.x, point.y - 1] != type)
                return CheckContinue3(new Vector2Int(point.x, point.y - 1), type, 0, cut);
            else
                return CheckContinue3(new Vector2Int(point.x, point.y - 1), type, 1, cut);
        }

        return true;
    }

    public bool CheckContinue3Type2(Vector2Int point, int type1, int type2)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < size;
        bool upIn = point.y + 1 < size;
        bool downIn = point.y - 1 > -1;
        int count;
        if (leftIn)
        {
            count = 0;
            if (map[point.x - 1, point.y] != type1 && map[point.x - 1, point.y] != type1)
            {
                if (upIn && (map[point.x - 1, point.y + 1] == type1 || map[point.x - 1, point.y + 1] == type2))
                    count++;
                if (downIn && (map[point.x - 1, point.y - 1] == type1 || map[point.x - 1, point.y - 1] == type2))
                    count++;
            }
            else
            {
                count++;
                if (upIn && (map[point.x - 1, point.y + 1] == type1 || map[point.x - 1, point.y + 1] == type2))
                    count++;
                if (downIn && (map[point.x - 1, point.y - 1] == type1 || map[point.x - 1, point.y - 1] == type2))
                    count++;
            }
            if (count > 3)
                return false;
        }

        if (rightIn)
        {
            count = 0;
            if (map[point.x + 1, point.y] != type1)
            {
                if (upIn && (map[point.x + 1, point.y + 1] == type1 || map[point.x + 1, point.y + 1] == type2))
                    count++;
                if (downIn && (map[point.x + 1, point.y - 1] == type1 || map[point.x + 1, point.y - 1] == type2))
                    count++;
            }
            else
            {
                count++;
                if (upIn && (map[point.x + 1, point.y + 1] == type1 || map[point.x + 1, point.y + 1] == type2))
                    count++;
                if (downIn && (map[point.x + 1, point.y - 1] == type1 || map[point.x + 1, point.y - 1] == type2))
                    count++;
            }
            if (count > 3)
                return false;
        }
        if (upIn)
        {
            count = 0;
            if (map[point.x, point.y + 1] != type1)
            {
                if (leftIn && (map[point.x - 1, point.y + 1] == type1 || map[point.x - 1, point.y + 1] == type2))
                    count++;
                if (rightIn && (map[point.x + 1, point.y + 1] == type1 || map[point.x + 1, point.y + 1] == type2))
                    count++;
            }
            else
            {
                count++;
                if (leftIn && (map[point.x - 1, point.y + 1] == type1 || map[point.x - 1, point.y + 1] == type2))
                    count++;
                if (rightIn && (map[point.x + 1, point.y + 1] == type1 || map[point.x + 1, point.y + 1] == type2))
                    count++;
            }
            if (count > 3)
                return false;
        }
        if (downIn)
        {
            count = 0;
            if (map[point.x, point.y - 1] != type1)
            {
                if (leftIn && (map[point.x - 1, point.y - 1] == type1 || map[point.x - 1, point.y - 1] == type2))
                    count++;
                if (rightIn && (map[point.x + 1, point.y - 1] == type1 || map[point.x + 1, point.y - 1] == type2))
                    count++;
            }
            else
            {
                count++;
                if (leftIn && (map[point.x - 1, point.y - 1] == type1 || map[point.x - 1, point.y - 1] == type2))
                    count++;
                if (rightIn && (map[point.x + 1, point.y - 1] == type1 || map[point.x + 1, point.y - 1] == type2))
                    count++;
            }
            if (count > 3)
                return false;
        }

        return true;
    }

    public bool CheckContinueStright(Vector2Int point, int length, int type)
    {
        int count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x - i > -1 && map[point.x - i, point.y] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x + i < size && map[point.x + i, point.y] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y + i < size && map[point.x, point.y + i] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y - i > -1 && map[point.x, point.y - i] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        int end = length / 2;
        if (length % 2 == 1)
            end++;

        for (int i = -length / 2; i < end; i++)
        {
            if (point.x + i > -1 && point.x + i < size && map[point.x + i, point.y] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = -length / 2; i < end; i++)
        {
            if (point.y + i > -1 && point.y + i < size && map[point.x, point.y + i] == type)
                count++;
        }
        if (count > length - 2)
            return false;

        return true;
    }

    public bool CheckContinueStrightType2(Vector2Int point, int length, int type1, int type2)
    {
        int count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x - i > -1 && (map[point.x - i, point.y] == type1 || map[point.x - i, point.y] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x + i < size && (map[point.x + i, point.y] == type1 || map[point.x + i, point.y] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y + i < size && (map[point.x, point.y + i] == type1 || map[point.x, point.y + i] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y - i > -1 && (map[point.x, point.y - i] == type1 || map[point.x, point.y - i] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        int end = length / 2;
        if (length % 2 == 1)
            end++;

        for (int i = -length / 2; i < end; i++)
        {
            if (point.x + i > -1 && point.x + i < size && (map[point.x + i, point.y] == type1 || map[point.x + i, point.y] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        count = 0;
        for (int i = -length / 2; i < end; i++)
        {
            if (point.y + i > -1 && point.y + i < size && (map[point.x, point.y + i] == type1 || map[point.x, point.y + i] == type2))
                count++;
        }
        if (count > length - 2)
            return false;

        return true;
    }

    public bool CheckAB(Vector2Int point)
    {
        if(CountAdjacent4Tile(point) == 4)
        {
            bool left = point.x - 1 > -1;
            bool right = point.x + 1 < size;
            bool up = point.y + 1 < size;
            bool down = point.y - 1 > -1;

            if (CountAdjacent4Tile(new Vector2Int(point.x, point.y + 1)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x, point.y - 1)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x - 1, point.y)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x + 1, point.y)) == 4)
                return false;

            if (CountAdjacent4Tile(new Vector2Int(point.x - 1, point.y + 1)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x - 1, point.y - 1)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x + 1, point.y + 1)) == 4)
                return false;
            if (CountAdjacent4Tile(new Vector2Int(point.x + 1, point.y - 1)) == 4)
                return false;
        }
        return true;
    }


    public bool CheckTwoSecretTile(Vector2Int point)
    {
        if (point.x - 2 > -1 && map[point.x - 2, point.y] == 0)
            return false;
        if (point.x + 2 < size && map[point.x + 2, point.y] == 0)
            return false;
        if (point.y + 2 < size && map[point.x, point.y + 2] == 0)
            return false;
        if (point.y - 2 > -1 && map[point.x, point.y - 2] == 0)
            return false;

        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < size;
        bool up = point.y + 1 < size;
        bool down = point.y - 1 > -1;

        if (left && up && map[point.x - 1, point.y + 1] == 0)
            return false;
        if (right && down && map[point.x + 1, point.y - 1] == 0)
            return false;
        if (up && right && map[point.x + 1, point.y + 1] == 0)
            return false;
        if (down && left && map[point.x - 1, point.y + -1] == 0)
            return false;

        return true;
    }

    public int GetNeedEndTileAmount()
    {
        int count = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if(map[i, j] == 1)
                {
                    count++;
                }
            }
        }

        switch (floor)
        {
            case 1:
                if(count < 2)
                    return 2 - count;
                break;
            case 2:
                if (count < 3)
                    return 3 - count;
                break;
            case 3:
                if (count < 4)
                    return 4 - count;
                break;
            case 4:
                if (count < 6)
                    return 6 - count;
                break;
            case 5:
                if (count < 8)
                    return 8 - count;
                break;
            case 6:
                if (count < 10)
                    return 10 - count;
                break;
        }

        return 0;
    }

}