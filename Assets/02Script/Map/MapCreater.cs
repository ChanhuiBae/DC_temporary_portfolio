using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreater : MonoBehaviour
{
    private Tilemap roadMap;
    private Tilemap backgroundMap;

    private TileBase gress;
    private TileBase tile;
    private TileBase pot;
    private TileBase box;
    private TileBase lattern;
    private TileBase cube;
    private TileBase god;
    private TileBase rune;
    private TileBase stone;
    private TileBase brick;

    [SerializeField]
    private int turn;
    private bool access = false;
    private MapData map = new MapData();

    [SerializeField]
    private int tileCount;
    [SerializeField]
    private int maxTile;


    public void CreateMap(int floor)
    {
        if (!map.SetSize(floor))
            return;
        access = false;
        map.map = new int[map.size, map.size];
        map.findMap = new bool[map.size, map.size];
        map.turn = new int[map.size, map.size];
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
                map.turn[i, j] = 0;
                map.goneMap[i, j] = 0;
            }
        }

        tileCount = 0;
        turn = 0;
        map.direction = Direction.None;
        maxTile = Random.Range(map.min, map.max);

        // starting area
        int half = map.size / 2;    
        if(map.size % 2 == 0)
            map.startPosition = new Vector2Int(Random.Range(half, half+2), Random.Range(half, half + 2));
        else
            map.startPosition = new Vector2Int(half,half);
        map.position = map.startPosition;
        map.map[map.startPosition.x, map.startPosition.y] = 0;
        map.turn[map.position.x, map.position.y] = 0;
        ++map.goneMap[map.position.x, map.position.y];
        tileCount++;
        PickPath(map.position);
        StartCoroutine(SetNext());
    }

    private IEnumerator SetNext()
    {
        while (tileCount < maxTile)
        {
            yield return null;
            if (turn > 600)
            {
                break;
            }

            GetNextPosition();
        }

        if (tileCount < map.min || map.GetNeedEndTileAmount() > 0)
        {
            CreateMap(map.floor);
        }
        else
        {
            CheckTileCase();
        }
    }

    private void GetNextPosition()
    {
        int turn = 600;
        Vector2Int next = map.position;
        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if (map.turn[i, j] > 0 && map.turn[i, j] <= turn && map.goneMap[i, j] == 0)
                {
                    if (map.turn[i, j] <= turn)
                    {
                        turn = map.turn[i, j];
                        next = new Vector2Int(i, j);
                    }
                }
            }
        }

        if (next == map.position)
        {
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if (map.turn[i, j] > 0 && map.goneMap[i, j] == 0)
                    {
                        turn = map.turn[i, j];
                        next = new Vector2Int(i, j);
                    }
                }
            }
        }

        map.position = next;
        PickPath(map.position);
        Move();
    }
    private void Move()
    {
        if (tileCount >= maxTile)
            return;

        UpdateMap(map.position);
        CheckArea(new Vector2Int(map.position.x, map.position.y));
        map.goneMap[map.position.x, map.position.y]++;
    }


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
            && map.CheckConnect2(point)
            && map.CheckAB(point))
            return true;
        return false;
    }

    private void PickPath(Vector2Int point)
    {
        turn++;
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
                if(pathCase + tileCount >= maxTile)
                {
                    pathCase = pathCase + tileCount - maxTile;
                }

                bool isUsed = false;
                int pickCount = 0;
                int first = -1;
                int second = -1;
                int third = -1;
                int pathCount;

                if (turn < 2)
                    pathCount = Random.Range(2, pathCase + 1);
                else
                    pathCount = Random.Range(1, pathCase + 1);
               
                while (pickCount < pathCount)
                {
                    int pick = Random.Range(0, 4);
                    if (pick != first && pick != second && pick != third)
                    {
                        if (pick == 0 && left)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x - 1, point.y));
                            map.turn[point.x - 1, point.y] = turn;
                        }
                        else if (pick == 1 && right)
                        {
                            isUsed = true;

                            UpdateMap(new Vector2Int(point.x + 1, point.y));
                            map.turn[point.x + 1, point.y] = turn;
                        }
                        else if (pick == 2 && up)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x, point.y + 1));
                            map.turn[point.x, point.y + 1] = turn;
                        }
                        else if (pick == 3 && down)
                        {
                            isUsed = true;
                            UpdateMap(new Vector2Int(point.x, point.y - 1));
                            map.turn[point.x, point.y - 1] = turn;
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

    private bool CreateSecretTile()
    {
        for (int k = 4; k > 0; k--)
        {
            for (int i = 0; i < map.size; i++)
            {
                for (int j = 0; j < map.size; j++)
                {
                    if ((map.map[i, j] < 0 || map.map[i, j] > (int)TileType.Boss)
                        && map.CountDiagonalTile(new Vector2Int(i, j)) == k
                        && map.CheckTwoSecretTile(new Vector2Int(i, j)))
                    {
                        map.map[i, j] = 0;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CheckSecretTile()
    {
        int secretCount = 0;
        int secret = TileData.GetTileMin(map.floor, tileCount, TileType.Secret);

        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((map.map[i, j] < 1 || map.map[i, j] > (int)TileType.Monster)
                    && map.CountAdjacent4Tile(new Vector2Int(i, j)) == 4
                    && map.CountDiagonalTile(new Vector2Int(i, j)) == 4
                    && map.CheckTwoSecretTile(new Vector2Int(i, j)))
                {
                    map.map[i, j] = 0;
                    secretCount++;
                }
            }
        }
        while (secretCount < secret)
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
                    int distance = map.GetMinDistance(new Vector2Int(i, j), map.startPosition);
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
            return false;
        }
        return false;
    }

    private bool CreateMonsterTile()
    {
        int count = 0;
        int[,] arr = new int[tileCount, 2];
        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((i != map.startPosition.x || j != map.startPosition.y)
                    && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                    && map.CheckContinueStright(new Vector2Int(i, j), 4, (int)TileType.Monster)
                    && map.CheckContinue4(new Vector2Int(i, j), (int)TileType.Monster, 4))
                {
                    arr[count, 0] = i;
                    arr[count,1] = j;
                    count++;
                    map.map[i,j] = (int)TileType.Monster;
                }
            }
        }
        
        int min = TileData.GetTileMin(map.floor, tileCount, TileType.Monster);
        int max = TileData.GetTileMax(map.floor, tileCount, TileType.Monster);
        
        if (count < min)
            return false;
        else if(count > max)
        {
            int delta = count - Random.Range(min, max + 1);

            for (int i = 0; i < delta; i++)
            {
                int random = Random.Range(0, count);

                int value = map.CountAdjacent4Tile(new Vector2Int(arr[random, 0], arr[random, 1]));
                if (value > 0)
                    map.map[arr[random, 0], arr[random, 1]] = value;
                else
                    map.map[arr[random, 0], arr[random, 1]] = -1;

                // change random values to last data.
                arr[random, 0] = arr[count - 1, 0];
                arr[random, 1] = arr[count - 1, 1];

                count--;
            }
        }
        return true;
    }

    private bool CreateMiddleBossTile()
    {
        int count = 0;
        int[,] arr = new int[tileCount, 2];
        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((i != map.startPosition.x || j != map.startPosition.y)
                    && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                    && map.GetMinDistance(new Vector2Int(i, j), map.startPosition) > 2)
                {
                    bool check = true;
                    for(int k = 0; k < count; k++)
                    {
                        if(map.GetMinDistance(new Vector2Int(i,j), new Vector2Int(arr[k,0], arr[k,1])) < 4)
                        {
                            check = false;
                            break;
                        }
                    }
                    if (check)
                    {
                        arr[count, 0] = i;
                        arr[count, 1] = j;
                        count++;
                        map.map[i, j] = (int)TileType.MiddleBoss;
                    }
                }
            }
        }

        int min = TileData.GetTileMin(map.floor, tileCount, TileType.MiddleBoss);
        int max = TileData.GetTileMax(map.floor, tileCount, TileType.MiddleBoss);

        if (count < min)
            return false;
        else if (count > max)
        {
            int delta = count - Random.Range(min, max + 1);

            for (int i = 0; i < delta; i++)
            {
                int random = Random.Range(0, count);

                int value = map.CountAdjacent4Tile(new Vector2Int(arr[random, 0], arr[random, 1]));
                if (value > 0)
                    map.map[arr[random, 0], arr[random, 1]] = value;
                else
                    map.map[arr[random, 0], arr[random, 1]] = -1;

                // change random values to last data.
                arr[random, 0] = arr[count - 1, 0];
                arr[random, 1] = arr[count - 1, 1];
                count--;
            }
        }
        return true;
    }


    private bool CreateChestTiles()
    {
        int count = 0;
        int[,] arr = new int[tileCount, 2];

        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((i != map.startPosition.x || j != map.startPosition.y)
                    && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                    && map.CheckContinueStright(new Vector2Int(i, j), 3, (int)TileType.Chest)
                    && map.CheckContinue3(new Vector2Int(i, j), (int)TileType.Chest, 0, 3))
                {
                    arr[count, 0] = i;
                    arr[count, 1] = j;
                    count++;
                    map.map[i, j] = (int)TileType.Chest;
                }
            }
        }
        int min = TileData.GetTileMin(map.floor, tileCount, TileType.Chest);
        int max = TileData.GetTileMax(map.floor, tileCount, TileType.Chest);

        if (count < min)
            return false;
        else if (count > max)
        {
            int delta = count - Random.Range(min, max + 1);

            for (int i = 0; i < delta; i++)
            {
                int random = Random.Range(0, count);

                int value = map.CountAdjacent4Tile(new Vector2Int(arr[random, 0], arr[random, 1]));
                if (value > 0)
                    map.map[arr[random, 0], arr[random, 1]] = value;
                else
                    map.map[arr[random, 0], arr[random, 1]] = -1;

                // change random values to last data.
                arr[random, 0] = arr[count - 1, 0];
                arr[random, 1] = arr[count - 1, 1];

                count--;
            }
        }

        for (int i = 0; i < count; i++)
        {
            map.map[arr[count, 0], arr[count, 1]] = 9;
        }
        return true;
    }
    private bool CreateEventTile()
    {
        int count = 0;
        int[,] arr = new int[tileCount, 2];


        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if ((i != map.startPosition.x || j != map.startPosition.y)
                    && map.map[i, j] > 0 && map.map[i, j] < (int)TileType.Boss
                    && map.CheckContinueStrightType2(new Vector2Int(i,j), 3, (int)TileType.PositiveEvent, (int)TileType.Chest)
                    && map.CheckContinue3Type2(new Vector2Int(i, j), (int)TileType.PositiveEvent, (int)TileType.Chest))
                {
                    arr[count, 0] = i;
                    arr[count, 1] = j;
                    count++;
                    map.map[i, j] = (int)TileType.PositiveEvent;
                }
            }
        }

        int min = TileData.GetTileMin(map.floor, tileCount, TileType.PositiveEvent);
        int max = TileData.GetTileMax(map.floor, tileCount, TileType.PositiveEvent);

        if (count < min)
            return false;
        else if (count > max)
        {
            int delta = count - Random.Range(min, max + 1);

            for (int i = 0; i < delta; i++)
            {
                int random = Random.Range(0, count);

                int value = map.CountAdjacent4Tile(new Vector2Int(arr[random, 0], arr[random, 1]));
                if (value > 0)
                    map.map[arr[random, 0], arr[random, 1]] = value;
                else
                    map.map[arr[random, 0], arr[random, 1]] = -1;

                // change random values to last data.
                arr[random, 0] = arr[count - 1, 0];
                arr[random, 1] = arr[count - 1, 1];

                count--;
            }
        }

        if (count == 1)
        {
            map.map[arr[0, 0], arr[0, 1]] = 10;
        }
        else
        {
            map.map[arr[0, 0], arr[0, 1]] = 10;
            map.map[arr[1, 0], arr[1, 1]] = 11;
            int positive = 0;
            int negative = 0;
            for (int i = 2; i < count; i++)
            {
                int random = Random.Range(10, 12);
                if(random == 10)
                    positive++;
                else
                    negative++;

                float p = positive / count;
                float n = negative / count;
                if(p > n * 3)
                    map.map[arr[count, 0], arr[count, 1]] = 11;
                else if(n > p * 3)
                    map.map[arr[count, 0], arr[count, 1]] = 10;
                else
                    map.map[arr[count, 0], arr[count, 1]] = random;
            }
        }
        return true;
    }

private void CheckTileCase() // Create Tile cases.
    {
        if (CreateBossTile()
            && CheckSecretTile()
            && CreateMiddleBossTile()
            && CreateMonsterTile()
            && CreateChestTiles()
            && CreateEventTile()
            )
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
            transform.GetChild(0).position = new Vector3(map.startPosition.x + 0.5f, map.startPosition.y + 0.5f, 0);
            DrawMap();
        }
        else
        {
            CreateMap(map.floor);
        }
    }


    private void DrawMap()
    {
        for (int i = 0; i < map.size; i++)
        {
            for (int j = 0; j < map.size; j++)
            {
                if (map.map[i, j] > 1)
                {
                    backgroundMap.SetTile(new Vector3Int(i, j, 0), gress);
                }
                
                if (map.map[i, j] == (int)TileType.Boss)
                {
                    roadMap.SetTile(new Vector3Int(i, j, 0), cube);
                }
                else if (map.map[i, j] == (int)TileType.Monster)
                {
                    backgroundMap.SetTile(new Vector3Int(i, j, 0), tile);
                }
                else if (map.map[i, j] == (int)TileType.Chest)
                {
                    roadMap.SetTile(new Vector3Int(i, j, 0), box);
                }
                else if (map.map[i, j] == (int)TileType.MiddleBoss)
                {
                    roadMap.SetTile(new Vector3Int(i, j, 0), lattern);
                }
                else if (map.map[i, j] == (int)TileType.PositiveEvent)
                {
                    roadMap.SetTile(new Vector3Int(i, j, 0), rune);
                }
                else if (map.map[i, j] == (int)TileType.NegativeEvent)
                {
                    roadMap.SetTile(new Vector3Int(i, j, 0), stone);
                }

            }
        }
    }

    private void Awake()
    {
        if (transform.childCount == 3)
        {
            if (transform.GetChild(1).TryGetComponent<Tilemap>(out roadMap)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out backgroundMap))
            {
                gress = backgroundMap.GetTile(Vector3Int.up);
                brick = backgroundMap.GetTile(Vector3Int.zero);
                tile = backgroundMap.GetTile(Vector3Int.left);
                box = backgroundMap.GetTile(Vector3Int.right);
                rune = backgroundMap.GetTile(new Vector3Int(3, 1, 0));
                pot = backgroundMap.GetTile(new Vector3Int(-1, 1, 0));
                god = backgroundMap.GetTile(new Vector3Int(2, 1, 0));
                cube = backgroundMap.GetTile(new Vector3Int(1, 1, 0));
                lattern = backgroundMap.GetTile(new Vector3Int(2, 0, 0));
                stone = backgroundMap.GetTile(new Vector3Int(3, 0, 0));
            }

            roadMap.ClearAllTiles();
            backgroundMap.ClearAllTiles();

            CreateMap(3);
        }
    }

}
