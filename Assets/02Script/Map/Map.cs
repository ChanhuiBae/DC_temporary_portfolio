using System.Collections;
using UnityEngine;

public class Map : MonoBehaviour
{
    private bool access = false;
    private MapData map = new MapData();

    [SerializeField]
    private int turn;
    [SerializeField]
    private int tileCount;
    [SerializeField]
    private int maxTile;

    #region setter & getter
    public bool Access
    {
        get => access;
        set => access = value;
    }

    public MapData Mapdata
    {
        get => map;
        set => map = value;
    }

    public Vector2Int Start
    {
        get => map.startPosition;
    }

    public int MapSize
    {
        get => map.size;
    }

    public int TileCount
    {
        get => tileCount;
    }

    public int MinTile
    {
        get => map.min;
    }

    public int Turn
    {
        get => turn;
    }

    public Direction GetDirection()
    {
        if (access)
            return map.direction;
        return Direction.None;
    }

    public Vector2Int GetPosition()
    {
        if (access)
            return map.position;
        return new Vector2Int(-1, -1);
    }

    public int GetMap(int i, int j)
    {
        if (access)
            return map.map[i, j];
        return -1;
    }
    public int GetCurrentMap()
    {
        if (access)
            return map.map[map.position.x, map.position.y];
        return -1;
    }

    public bool GetFind(int i, int j)
    {
        if (access)
            return map.findMap[i, j];
        return false;
    }
    public bool GetKnown(int i, int j)
    {
        if (access)
            return map.knownMap[i, j];
        return false;
    }

    public int GetGone(int i, int j)
    {
        if (access)
            return map.goneMap[i, j];
        return -1;
    }

    public bool SetGonefalse()
    {
        if (access)
        {
            map.goneMap[map.position.x, map.position.y] = 0;
            return true;
        }
        return false;
    }

    public bool GetBlownUp(int i, int j)
    {
        if (access)
            return map.blownupMap[i, j];
        return false;
    }

    public void ClearMap()
    {
        if (access)
        {
            map.map[map.position.x, map.position.y] = (int)TileType.Merchant;
        }
    }

    #endregion

    public Map()
    {
        tileCount = 0;
        access = false;
    }

    #region CreateMap
    private bool UpdateMap(Vector2Int point)
    {
        if (map.map[point.x, point.y] == -1 && tileCount < maxTile)
        {
            tileCount++;
        }

        bool leftOut = point.x - 1 > -1;
        bool rightOut = point.x + 1 < map.size;
        bool upOut = point.y + 1 < map.size;
        bool downOut = point.y - 1 > -1;
        bool left = false;
        bool right = false;
        bool up = false;
        bool down = false;
        if (point.x - 1 > -1)
        {
            left = map.map[point.x - 1, point.y] > -1;
        }
        if (point.x + 1 < map.size)
        {
            right = map.map[point.x + 1, point.y] > -1;
        }
        if (point.y + 1 < map.size)
        {
            up = map.map[point.x, point.y + 1] > -1;
        }
        if (point.y - 1 > -1)
        {
            down = map.map[point.x, point.y - 1] > -1;
        }

        map.map[point.x, point.y] = 0;
        if (leftOut && left)
            map.map[point.x, point.y] += 1;
        if (rightOut && right)
            map.map[point.x, point.y] += 1;
        if (upOut && up)
            map.map[point.x, point.y] += 1;
        if (downOut && down)
            map.map[point.x, point.y] += 1;

        if (left)
        {
            map.map[point.x - 1, point.y] = 1;
            if (point.x - 2 > -1 && map.map[point.x - 2, point.y] > -1)
                map.map[point.x - 1, point.y] += 1;
            if (upOut && map.map[point.x - 1, point.y + 1] > -1)
                map.map[point.x - 1, point.y] += 1;
            if (downOut && map.map[point.x - 1, point.y - 1] > -1)
                map.map[point.x - 1, point.y] += 1;
        }

        if (right)
        {
            map.map[point.x + 1, point.y] = 1;
            if (point.x + 2 < map.size && map.map[point.x + 2, point.y] > -1)
                map.map[point.x + 1, point.y] += 1;
            if (upOut && map.map[point.x + 1, point.y + 1] > -1)
                map.map[point.x + 1, point.y] += 1;
            if (downOut && map.map[point.x + 1, point.y - 1] > -1)
                map.map[point.x + 1, point.y] += 1;
        }

        if (up)
        {
            map.map[point.x, point.y + 1] = 1;
            if (point.y + 2 < map.size && map.map[point.x, point.y + 2] > -1)
                map.map[point.x, point.y + 1] += 1;
            if (leftOut && map.map[point.x - 1, point.y + 1] > -1)
                map.map[point.x, point.y + 1] += 1;
            if (rightOut && map.map[point.x + 1, point.y + 1] > -1)
                map.map[point.x, point.y + 1] += 1;
        }

        if (down)
        {
            map.map[point.x, point.y - 1] = 1;
            if (point.y - 2 > -1 && map.map[point.x, point.y - 2] > -1)
                map.map[point.x, point.y - 1] += 1;
            if (leftOut && map.map[point.x - 1, point.y - 1] > -1)
                map.map[point.x, point.y - 1] += 1;
            if (rightOut && map.map[point.x + 1, point.y - 1] > -1)
                map.map[point.x, point.y - 1] += 1;
        }

        return true;
    }

    private bool CheckArea(Vector2Int point)
    {
        if (map.map[point.x, point.y] == -1
            && !map.findMap[point.x, point.y]
            && tileCount < maxTile
            && map.CheckConnect1(point)
            && map.CheckConnect2(point))
            return true;
        return false;
    }

    private void PickPath(Vector2Int point)
    {
        map.direction = Direction.None;
        if (!map.findMap[point.x, point.y] && UpdateMap(point))
        {
            bool left = false;
            bool right = false;
            bool up = false;
            bool down = false;

            // left
            if (point.x - 1 > -1 && CheckArea(new Vector2Int(point.x - 1, point.y)))
            {
                left = true;
            }
            // right
            if (point.x + 1 < map.size && CheckArea(new Vector2Int(point.x + 1, point.y)))
            {
                right = true;
            }
            // up
            if (point.y + 1 < map.size && CheckArea(new Vector2Int(point.x, point.y + 1)))
            {
                up = true;
            }
            // down
            if (point.y - 1 > -1 && CheckArea(new Vector2Int(point.x, point.y - 1)))
            {
                down = true;
            }

            int pathCase = 0;
            if (left)
                pathCase++;
            if (right)
                pathCase++;
            if (up)
                pathCase++;
            if (down)
                pathCase++;

            if (pathCase == 0)
            {
                return;
            }
            else
            {
                bool isUsed = false;
                int pickCount = 0;
                int first = -1;
                int second = -1;
                int third = -1;
                int pathCount;

                if (turn == 0)
                    pathCount = Random.Range(2, pathCase + 1);
                else if(turn < 2)
                    pathCount = Random.Range(1, pathCase);
                else
                    pathCount = Random.Range(0, pathCase + 1);
               
                while (pickCount < pathCount)
                    {
                        int pick = Random.Range(0, 4);
                        if (pick != first && pick != second && pick != third)
                        {
                            if (pick == 0 && left)
                            {
                                isUsed = true;
                                UpdateMap(new Vector2Int(point.x - 1, point.y));
                            }
                            else if (pick == 1 && right)
                            {
                                isUsed = true;

                                UpdateMap(new Vector2Int(point.x + 1, point.y));
                            }
                            else if (pick == 2 && up)
                            {
                                isUsed = true;
                                UpdateMap(new Vector2Int(point.x, point.y + 1));
                            }
                            else if (pick == 3 && down)
                            {
                                isUsed = true;
                                UpdateMap(new Vector2Int(point.x, point.y - 1));
                            }

                            if (isUsed)
                            {
                                isUsed = false;
                                pickCount++;
                                if (first == -1)
                                {
                                    first = pick;
                                }
                                else if (second == -1)
                                {
                                    second = pick;
                                }
                                else if (third == -1)
                                {
                                    third = pick;
                                }
                            }
                        }
                    }
            }
        }
    }


    private void MoveDirection()
    {
        if (tileCount >= maxTile)
            return;

        switch (map.direction)
        {
            case Direction.Left:
                if (map.position.x - 1 > -1 && map.map[map.position.x - 1, map.position.y] > -1)
                {
                    turn++;
                    map.position.x -= 1;
                    if (UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y)))
                        PickPath(new Vector2Int(map.position.x, map.position.y));
                }
                else
                {
                    map.direction = Direction.None;
                }
                break;
            case Direction.Right:
                if (map.position.x + 1 < map.size && map.map[map.position.x + 1, map.position.y] > -1)
                {
                    turn++;
                    map.position.x += 1;
                    if (UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y)))
                        PickPath(new Vector2Int(map.position.x, map.position.y));
                }
                else
                {
                    map.direction = Direction.None;
                }
                break;
            case Direction.Up:
                if (map.position.y + 1 < map.size && map.map[map.position.x, map.position.y + 1] > -1)
                {
                    turn++;
                    map.position.y += 1;
                    if (UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y)))
                        PickPath(new Vector2Int(map.position.x, map.position.y));
                }
                else
                {
                    map.direction = Direction.None;
                }
                break;
            case Direction.Down:
                if (map.position.y - 1 > -1 && map.map[map.position.x, map.position.y - 1] > -1)
                {
                    turn++;
                    map.position.y -= 1;
                    if (UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y)))
                        PickPath(new Vector2Int(map.position.x, map.position.y));
                }
                else
                {
                    map.direction = Direction.None;
                }
                break;
        }

        if (map.direction != Direction.None)
        {
            if (map.position.x - 1 > -1 && UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x - 1, map.position.y)))
                PickPath(new Vector2Int(map.position.x - 1, map.position.y));
            if (map.position.x + 1 < map.size && UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x + 1, map.position.y)))
                PickPath(new Vector2Int(map.position.x + 1, map.position.y));
            if (map.position.y + 1 < map.size && UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y + 1)))
                PickPath(new Vector2Int(map.position.x, map.position.y + 1));
            if (map.position.y - 1 > -1 && UpdateMap(map.position) && CheckArea(new Vector2Int(map.position.x, map.position.y - 1)))
                PickPath(new Vector2Int(map.position.x, map.position.y - 1));
        }

        map.UpdateFindMap(map.position);
    }


    public void CreateMap(int floor)
    {
        if (!map.SetSize(floor))
            return;
        access = false;
        map.map = new int[map.size, map.size];
        map.findMap = new bool[map.size, map.size];
        map.knownMap = new bool[map.size, map.size];
        map.goneMap = new int[map.size, map.size];
        map.blownupMap = new bool[map.size, map.size];
        map.searchMap = new bool[map.size, map.size];

        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                map.map[i, j] = -1;
                map.findMap[i, j] = false;
            }
        }

        tileCount = 0;
        turn = 0;
        map.direction = Direction.None;
        maxTile = Random.Range(map.min, map.max);

        // starting area
        if(map.size > 10)
        {
            map.startPosition = new Vector2Int(Random.Range(map.size / 2 - 2, map.size / 2 + 2), Random.Range(map.size / 2 - 2, map.size / 2 + 2));
        }
        else
        {
            map.startPosition = new Vector2Int(Random.Range(map.size / 2 - 1, map.size / 2 + 1), Random.Range(map.size / 2 - 1, map.size / 2 + 1));
        }
        map.position = map.startPosition;
        map.map[map.startPosition.x, map.startPosition.y] = 0;
        tileCount++;
        PickPath(map.position);
        StartCoroutine(SetDirection());
    }

    private IEnumerator SetDirection()
    {
        while (tileCount < maxTile)
        {
            yield return null;
            if (turn > 600)
            {
                break;
            }

            switch (Random.Range(0, 4))
            {
                case 0:
                    map.direction = Direction.Left;
                    MoveDirection();
                    break;
                case 1:
                    map.direction = Direction.Right;
                    MoveDirection();
                    break;
                case 2:
                    map.direction = Direction.Up;
                    MoveDirection();
                    break;
                case 3:
                    map.direction = Direction.Down;
                    MoveDirection();
                    break;
            }
        }

        if (tileCount < map.min)
        {
            CreateMap(map.floor);
        }
        else
        {
            CheckTileCase();
        }
    }


    private bool CreateSecretTile()
    {
        for (int k = 4; k > 0; k--)
        {
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if ((map.map[i, j] < 0 || map.map[i, j] > (int)TileType.Boss)
                        && Check4AdjacentTile(new Vector2Int(i, j)) == k
                        && CheckTwoSecretTile(new Vector2Int(i, j)))
                    {
                        map.map[i, j] = 0;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckTwoSecretTile(Vector2Int point)
    {
        if (point.x - 2 > -1 && map.map[point.x - 2, point.y] == 0)
            return false;
        if (point.x + 2 < map.size && map.map[point.x + 2, point.y] == 0)
            return false;
        if (point.y + 2 < map.size && map.map[point.x, point.y + 2] == 0)
            return false;
        if (point.y - 2 > -1 && map.map[point.x, point.y - 2] == 0)
            return false;

        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < map.size;
        bool up = point.y + 1 < map.size;
        bool down = point.y - 1 > -1;

        if (left && up && map.map[point.x - 1, point.y + 1] == 0)
            return false;
        if (right && down && map.map[point.x + 1, point.y - 1] == 0)
            return false;
        if (up && right && map.map[point.x + 1, point.y + 1] == 0)
            return false;
        if (down && left && map.map[point.x - 1, point.y + -1] == 0)
            return false;

        return true;
    }


    private int CountAdjacentTile(Vector2Int point)
    {
        int count = 0;
        bool left = point.x - 1 > -1;
        bool right = point.x + 1 < map.size;
        bool up = point.y + 1 < map.size;
        bool down = point.y - 1 > -1;

        if (left && up && map.map[point.x - 1, point.y + 1] > 0)
        {
            count++;
        }
        if (right && down && map.map[point.x + 1, point.y - 1] > 0)
        {
            count++;
        }
        if (up && right && map.map[point.x + 1, point.y + 1] > 0)
        {
            count++;
        }
        if (down && left && map.map[point.x - 1, point.y + -1] > 0)
        {
            count++;
        }
        return count;
    }

    private int Check4AdjacentTile(Vector2Int point)
    {
        if (point.x - 1 > -1 && point.x + 1 < map.size && point.y + 1 < map.size && point.y - 1 > -1)
        {
            int count = 0;

            if (map.map[point.x - 1, point.y] > 0 && map.map[point.x - 1, point.y] < (int)TileType.Monster)
                count++;
            if (map.map[point.x + 1, point.y] > 0 && map.map[point.x + 1, point.y] < (int)TileType.Monster)
                count++;
            if (map.map[point.x, point.y + 1] > 0 && map.map[point.x, point.y + 1] < (int)TileType.Monster)
                count++;
            if (map.map[point.x, point.y - 1] > 0 && map.map[point.x, point.y - 1] < (int)TileType.Monster)
                count++;

            return count;
        }
        return 0;
    }

    private bool CheckSecretTile()
    {
        int secretCount = 0;

        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((map.map[i, j] < 1 || map.map[i, j] > (int)TileType.Monster)
                    && Check4AdjacentTile(new Vector2Int(i, j)) == 4
                    && CheckTwoSecretTile(new Vector2Int(i, j))
                    && CountAdjacentTile(new Vector2Int(i, j)) == 4)
                {
                    map.map[i, j] = 0;
                    secretCount++;
                }
            }
        }

        int amount;
        if(map.floor < 2)
            amount = 1;
        else if(map.floor < 4)
            amount = 2;
        else if(map.floor < 6)
            amount = 3;
        else
            amount = 4;
        while (secretCount < amount)
        {
            if (CreateSecretTile())
                secretCount++;
        }
        return true;
    }

    private bool CreateBossTile()
    {
        int bossCount = 0;

        for (int i = 0; i < map.size && bossCount == 0; i++)
        {
            for (int j = 0; j < map.size && bossCount == 0; j++)
            {
                if (map.map[i, j] == 1)
                {
                    int distance = map.minDistance(new Vector2Int(i, j), map.startPosition);
                    if (distance > map.size / 2)
                    {
                        bossCount++;
                        map.map[i, j] = (int)TileType.Boss;
                        return true;
                    }
                }
            }
        }

        if (bossCount == 0)
        {
            CreateMap(map.floor);
            return false;
        }
        return false;
    }

    private bool CheckContinue4(Vector2Int point, int type)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < map.size;
        bool upIn = point.y + 1 < map.size;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map.map[point.x - 1, point.y] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x - 1, point.y), type))
                return false;
        }
        if (rightIn && map.map[point.x + 1, point.y] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x + 1, point.y), type))
                return false;
        }
        if (upIn && map.map[point.x, point.y + 1] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x, point.y + 1), type))
                return false;
        }
        if (downIn && map.map[point.x, point.y - 1] == type)
        {
            if (!CheckContinue3(new Vector2Int(point.x, point.y - 1), type))
                return false;
        }

        return true;
    }

    private bool CheckContinue3(Vector2Int point, int type)
    {
        bool leftIn = point.x - 1 > -1;
        bool rightIn = point.x + 1 < map.size;
        bool upIn = point.y + 1 < map.size;
        bool downIn = point.y - 1 > -1;

        if (leftIn && map.map[point.x - 1, point.y] == type)
        {
            if (upIn && map.map[point.x - 1, point.y + 1] == type)
                return false;
            if (downIn && map.map[point.x - 1, point.y - 1] == type)
                return false;
        }
        if (rightIn && map.map[point.x + 1, point.y] == type)
        {
            if (upIn && map.map[point.x + 1, point.y + 1] == type)
                return false;
            if (downIn && map.map[point.x + 1, point.y - 1] == type)
                return false;
        }
        if (upIn && map.map[point.x, point.y + 1] == type)
        {
            if (leftIn && map.map[point.x - 1, point.y + 1] == type)
                return false;
            if (rightIn && map.map[point.x + 1, point.y + 1] == type)
                return false;
        }
        if (downIn && map.map[point.x, point.y - 1] == type)
        {
            if (leftIn && map.map[point.x - 1, point.y - 1] == type)
                return false;
            if (rightIn && map.map[point.x + 1, point.y - 1] == type)
                return false;
        }

        return true;
    }


    private bool CheckContinueStright(Vector2Int point, int length, int type)
    {
        int count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x - i > -1 && map.map[point.x - i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.x + i < map.size && map.map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y + i < map.size && map.map[point.x, point.y + i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = 1; i < length; i++)
        {
            if (point.y - i > -1 && map.map[point.x, point.y - i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        int end = length / 2;
        if (length % 2 == 1)
            end++;

        for (int i = -length / 2; i < end; i++)
        {
            if (point.x + i > -1 && point.x + i < map.size && map.map[point.x + i, point.y] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        count = 0;
        for (int i = -length / 2; i < end; i++)
        {
            if (point.y + i > -1 && point.y + i < map.size && map.map[point.x, point.y + i] == type)
                count++;
        }
        if (count >= length - 1)
            return false;

        return true;
    }

    private bool CreateEventTile()
    {
        int count = 0;
        int eventTile;
        if (map.floor < 2)
            eventTile = Random.Range(3,5);
        else if (map.floor < 4)
            eventTile = Random.Range(5, 7);
        else if (map.floor < 6)
            eventTile = Random.Range(7, 9);
        else
            eventTile = Random.Range(9, 11);

        for (int k = 0; k < 5; k++)
        {
            if (count >= eventTile)
                break;
            for (int i = 0; i < map.size; i++)
            {
                if (count >= eventTile)
                    break;
                for (int j = 0; j < map.size; j++)
                {
                    if (count >= eventTile)
                        break;

                    if ((i != map.startPosition.x || j != map.startPosition.y)
                        && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 2, (int)TileType.NegativeEvent)
                        && CheckContinue4(new Vector2Int(i, j), (int)TileType.NegativeEvent))
                    {
                        map.map[i, j] = (int)TileType.NegativeEvent;
                        count++;
                    }
                }
            }
        }

        int positive = count / 2;

        count = 0;
        for (int k = 0; k < 5; k++)
        {
            if (count >= positive)
                break;
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if (count >= positive)
                    {
                        return true;
                    }

                    if (map.map[i, j] == (int)TileType.NegativeEvent
                        && Random.Range(0, 100) < 50)
                    {
                        map.map[i, j] = (int)TileType.PositiveEvent;
                        count++;
                    }
                }
            }
        }
        return true;
    }

    private bool CreateChestTiles()
    {
        int count = 0;
        int chest;
        if (map.floor < 2)
            chest = Random.Range(2, 4);
        else if (map.floor < 4)
            chest = Random.Range(3, 5);
        else if (map.floor < 6)
            chest = Random.Range(5, 7);
        else
            chest = Random.Range(6, 8);


        for (int k = 0; k < 5; k++)
        {
            if (count >= chest)
                break;
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if (count >= chest)
                        return true;

                    if ((i != map.startPosition.x || j != map.startPosition.y)
                        && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 2, (int)TileType.Chest)
                        && CheckContinue3(new Vector2Int(i, j), (int)TileType.Chest))
                    {
                        map.map[i, j] = (int)TileType.Chest;
                        count++;
                    }
                }
            }
        }

        if (count < chest)
            Debug.Log("Chest: " + count);

        return true;
    }

    private bool CreateMonsterTile()
    {
        int count = 0;
        int monster;
        if (map.floor < 2)
            monster = Random.Range(6, 10);
        else if (map.floor < 4)
            monster = Random.Range(7, 11);
        else if (map.floor < 6)
            monster = Random.Range(9, 12);
        else
            monster = Random.Range(10, 13);

        for (int k = 0; k < 5; k++)
        {
            if (count >= monster)
                break;
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if (count >= monster)
                        return true;

                    if ((i != map.startPosition.x || j != map.startPosition.y)
                        && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                        && Random.Range(0, 100) < 50
                        && CheckContinueStright(new Vector2Int(i, j), 2, (int)TileType.Monster)
                        && CheckContinue4(new Vector2Int(i, j), (int)TileType.Monster))
                    {
                        map.map[i, j] = (int)TileType.Monster;
                        count++;
                    }
                }
            }
        }

        if (count < monster)
            Debug.Log("Monster: " + count);

        return true;
    }

    private bool CreateMiddleBossTile()
    {
        int count = 0;
        int monster;
        if (map.floor < 2)
            monster = 2;
        else if (map.floor < 4)
            monster = 3;
        else
            monster = 4;

        for (int k = 0; k < 4; k++)
        {
            if (count >= monster)
                break;
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if (count >= monster)
                        return true;

                    if (map.map[i, j] == (int)TileType.Monster
                        && map.minDistance(new Vector2Int(i, j), map.startPosition) > 3
                        && CheckContinueStright(new Vector2Int(i, j), 4, (int)TileType.MiddleBoss)
                        && CheckContinue4(new Vector2Int(i, j), (int)TileType.MiddleBoss))
                    {
                        map.map[i, j] = (int)TileType.MiddleBoss;
                        count++;
                    }
                }
            }
        }

        if (count < monster)
            Debug.Log("Middle: " + count);

        return true;
    }

    /////////////////////// Create Tiles ///////////////////////////
    private void CheckTileCase() 
    {
        if (CreateBossTile() 
            && CheckSecretTile() 
            // && CreateMonsterTile() 
            && CreateMiddleBossTile()
            && CreateChestTiles() 
            && CreateEventTile())
        {
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    map.findMap[i, j] = false;
                    map.knownMap[i, j] = false;
                    map.goneMap[i, j] = 0;
                    map.blownupMap[i, j] = false;
                    map.searchMap[i, j] = false;
                }
            }

            map.UpdateFindMap(map.startPosition);
            map.position = map.startPosition;
            map.goneMap[map.startPosition.x, map.startPosition.y] = 1;
            map.knownMap[map.startPosition.x, map.startPosition.y] = true;
            map.direction = Direction.None;

            access = true;
        }
    }

    #endregion

    #region Exploration
    public bool MovePlayer(Direction direction)
    {
        map.direction = direction;
        switch (direction)
        {
            case Direction.Left:
                if (map.position.x - 1 > -1 && map.map[map.position.x - 1, map.position.y] > -1)
                {
                    if (map.map[map.position.x - 1, map.position.y] == (int)TileType.Secret)
                    {
                        if (!map.blownupMap[map.position.x, map.position.y])
                            return false;
                        else
                        {
                            if (map.position.x - 2 > -1)
                                map.blownupMap[map.position.x - 2, map.position.y] = true;
                            if (map.position.y + 1 < map.size)
                                map.blownupMap[map.position.x - 1, map.position.y + 1] = true;
                            if (map.position.y - 1 > -1)
                                map.blownupMap[map.position.x - 1, map.position.y - 1] = true;
                        }
                    }
                    map.position.x -= 1;
                    map.UpdateFindMap(map.position);
                    map.knownMap[map.position.x, map.position.y] = true;
                    ++map.goneMap[map.position.x, map.position.y];
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Right:
                if (map.position.x + 1 < map.size && map.map[map.position.x + 1, map.position.y] > -1)
                {
                    if (map.map[map.position.x + 1, map.position.y] == (int)TileType.Secret)
                    {
                        if (!map.blownupMap[map.position.x, map.position.y])
                            return false;
                        else
                        {
                            if (map.position.x + 2 < map.size)
                                map.blownupMap[map.position.x + 2, map.position.y] = true;
                            if (map.position.y + 1 < map.size)
                                map.blownupMap[map.position.x + 1, map.position.y + 1] = true;
                            if (map.position.y - 1 > -1)
                                map.blownupMap[map.position.x + 1, map.position.y - 1] = true;
                        }
                    }
                    map.position.x += 1;
                    map.UpdateFindMap(map.position);
                    map.knownMap[map.position.x, map.position.y] = true;
                    ++map.goneMap[map.position.x, map.position.y];
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Up:
                if (map.position.y + 1 < map.size && map.map[map.position.x, map.position.y + 1] > -1)
                {
                    if (map.map[map.position.x, map.position.y + 1] == (int)TileType.Secret)
                    {
                        if (!map.blownupMap[map.position.x, map.position.y])
                            return false;
                        else
                        {
                            if (map.position.x - 1 > -1)
                                map.blownupMap[map.position.x - 1, map.position.y + 1] = true;
                            if (map.position.x + 1 < map.size)
                                map.blownupMap[map.position.x + 1, map.position.y + 1] = true;
                            if (map.position.y + 2 < map.size)
                                map.blownupMap[map.position.x, map.position.y + 2] = true;
                        }
                    }
                    map.position.y += 1;
                    map.UpdateFindMap(map.position);
                    map.knownMap[map.position.x, map.position.y] = true;
                    ++map.goneMap[map.position.x, map.position.y];
                    return true;
                }
                else
                {
                    return false;
                }
            case Direction.Down:
                if (map.position.y - 1 > -1 && map.map[map.position.x, map.position.y - 1] > -1)
                {
                    if (map.map[map.position.x, map.position.y - 1] == (int)TileType.Secret)
                    {
                        if (!map.blownupMap[map.position.x, map.position.y])
                            return false;
                        else
                        {
                            if (map.position.x - 1 > -1)
                                map.blownupMap[map.position.x - 1, map.position.y - 1] = true;
                            if (map.position.x + 1 < map.size)
                                map.blownupMap[map.position.x + 1, map.position.y - 1] = true;
                            if (map.position.y - 2 > -1)
                                map.blownupMap[map.position.x, map.position.y - 2] = true;
                        }
                    }
                    map.position.y -= 1;
                    map.UpdateFindMap(map.position);
                    map.knownMap[map.position.x, map.position.y] = true;
                    ++map.goneMap[map.position.x, map.position.y];
                    return true;
                }
                else
                {
                    direction = Direction.None;
                    return false;
                }
            default:
                direction = Direction.None;
                return false;
        }
    }

    public int GetCurrentGone()
    {
        return map.goneMap[map.position.x, map.position.y];
    }
    public void UpdateKnownMap() // only used Reconnaissance scusses
    {
        if (map.position.x - 1 > -1 && map.map[map.position.x - 1, map.position.y] != 0)
            map.knownMap[map.position.x - 1, map.position.y] = true;
        if (map.position.x + 1 < map.size && map.map[map.position.x + 1, map.position.y] != 0)
            map.knownMap[map.position.x + 1, map.position.y] = true;
        if (map.position.y + 1 < map.size && map.map[map.position.x, map.position.y + 1] != 0)
            map.knownMap[map.position.x, map.position.y + 1] = true;
        if (map.position.y - 1 > -1 && map.map[map.position.x, map.position.y - 1] != 0)
            map.knownMap[map.position.x, map.position.y - 1] = true;
    }

    public bool CheckSearched()
    {
        if (map.searchMap[map.position.x, map.position.y])
        {
            if (map.position.x - 1 > -1 && map.map[map.position.x - 1, map.position.y] == 0)
                return false;
            if (map.position.x + 1 < map.size && map.map[map.position.x + 1, map.position.y] == 0)
                return false;
            if (map.position.y + 1 < map.size && map.map[map.position.x, map.position.y + 1] == 0)
                return false;
            if (map.position.y - 1 > -1 && map.map[map.position.x, map.position.y - 1] == 0)
                return true;
            return true;
        }
        return false;
    }

    public SearchEventType Search()
    {
        map.searchMap[map.position.x, map.position.y] = true;
        // check secrect tile
        if (map.position.x - 1 > -1 && map.map[map.position.x - 1, map.position.y] == 0)
        {
            map.findMap[map.position.x - 1, map.position.y] = true;
            return SearchEventType.FindSecrect;
        }
        if (map.position.x + 1 < map.size && map.map[map.position.x + 1, map.position.y] == 0)
        {
            map.findMap[map.position.x + 1, map.position.y] = true;
            return SearchEventType.FindSecrect;
        }
        if (map.position.y + 1 < map.size && map.map[map.position.x, map.position.y + 1] == 0)
        {
            map.findMap[map.position.x, map.position.y + 1] = true;
            return SearchEventType.FindSecrect;
        }
        if (map.position.y - 1 > -1 && map.map[map.position.x, map.position.y - 1] == 0)
        {
            map.findMap[map.position.x, map.position.y - 1] = true;
            return SearchEventType.FindSecrect;
        }

        // other events
        int result = Random.Range(0, 100);
        if (result < 20)
        {
            return SearchEventType.Monster;
        }
        else if (result < 50)
        {
            return SearchEventType.Monster;
        }
        else
        {
            return SearchEventType.None;
        }
    }

    public void BlownUp()
    {
        map.blownupMap[map.position.x, map.position.y] = true;
    }
    #endregion

}
