using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapPainter : MonoBehaviour
{
    private MapManager mapManager;
    private MapData mapData => GameManager.Inst.Exploration.map.Mapdata;

    private Tilemap frameMap;
    private Tilemap darkMap;
    private Tilemap wall3Map;
    private Tilemap wall2Map;
    private Tilemap wall1Map;
    private Tilemap shadowMap;
    private Tilemap groundMap;

    private TileBase[,] frame;
    private TileBase[,] dark;
    private TileBase[,] wall3;
    private TileBase[,] wall2;
    private TileBase[,] wall1;
    private TileBase[,] shadow;
    private TileBase[,] ground;

    public MapProps props;
   

    private void DrawRoom(int x, int y)
    {
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                if (i > 1 && i < 10)
                {
                    groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                }
                if (i > 1 && i < 10)
                {
                    wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                }
                if (i == 1 || i == 10)
                {
                    wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall3[i, j]);
                }
            }
        }
    }

    private void DrawLeftPath(int x, int y)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 3; j < 9; j++)
            {
                wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawRightPath(int x, int y)
    {
        for (int i = 10; i < 12; i++)
        {
            for (int j = 3; j < 9; j++)
            {
                wall1Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall1[i, j]);
                wall2Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall2[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawUpPath(int x, int y)
    {
        for (int i = 4; i < 8; i++)
        {
            wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), wall3[i, 11]);
            shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), shadow[i, 11]);
            groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + 11), ground[i, 11]);
        }
    }

    private void DrawDownPath(int x, int y)
    {
        for (int i = 4; i < 8; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                wall3Map.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), wall3[i, j]);
                shadowMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), shadow[i, j]);
                groundMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), ground[i, j]);
            }
        }
    }

    private void DrawUpNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 9), wall1[4, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 9), wall1[4, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 10), wall2[4, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 10), wall2[4, 2]);
    }
    private void DrawDownNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 1), wall1[4, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 1), wall1[4, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 2), wall2[4, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 2), wall2[4, 2]);
    }

    private void DeleteUpNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 9), wall1[5, 9]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 9), wall1[6, 9]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 10), wall2[5, 10]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 10), wall2[6, 10]);
    }
    private void DeleteDownNoGate(int x, int y)
    {
        wall1Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 1), wall1[5, 1]);
        wall1Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 1), wall1[6, 1]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 5, y * 12 + 2), wall2[5, 2]);
        wall2Map.SetTile(new Vector3Int(x * 12 + 6, y * 12 + 2), wall2[6, 2]);
    }
    private void DrawPathes(int i, int j, int size, Vector2Int v)
    {
        if (i - 1 > -1 && mapData.findMap[i - 1, j])
        {
            if (mapData.map[i - 1, j] > 0)
            {
                DrawLeftPath(i, j);
            }
            else if (mapData.map[i - 1, j] == 0 && mapData.goneMap[i - 1, j] > 0)
            {
                DrawLeftPath(i, j);
            }
        }

        if (i + 1 < size && mapData.findMap[i + 1, j])
        {
            if (mapData.map[i + 1, j] > 0)
            {
                DrawRightPath(i, j);
            }
            else if (mapData.map[i + 1, j] == 0 && mapData.goneMap[i + 1, j] > 0)
            {
                DrawRightPath(i, j);
            }
        }

        if (j + 1 < size && mapData.findMap[i, j + 1])
        {
            if (mapData.map[i, j + 1] > 0)
            {
                DrawUpPath(i, j);
                if (mapData.goneMap[i, j] > 0)
                {
                    props.DrawOpenedGate(v, true);
                }
                else
                {
                    props.DrawClosedGate(v, true);
                }
            }
            else if (mapData.map[i, j + 1] == 0 && mapData.goneMap[i, j + 1] > 0)
            {
                DrawUpPath(i, j);
                if (mapData.goneMap[i, j] > 0)
                {
                    props.DrawOpenedGate(v, true);
                }
                else
                {
                    props.DrawClosedGate(v, true);
                }
            }
            else
            {
                DrawUpNoGate(i, j);
            }
        }
        else
        {
            DrawUpNoGate(i, j);
        }


        if (j - 1 > -1 && mapData.findMap[i, j - 1])
        {
            if (mapData.map[i, j - 1] > 0)
            {
                DrawDownPath(i, j);
                if (mapData.goneMap[i, j] > 0)
                {
                    props.DrawOpenedGate(v, false);
                }
                else
                {
                    props.DrawClosedGate(v, false);
                }
            }
            else if (mapData.map[i, j - 1] == 0 && mapData.goneMap[i, j - 1] > 0)
            {
                DrawDownPath(i, j);
                if (mapData.goneMap[i, j] > 0)
                {
                    props.DrawOpenedGate(v, false);
                }
                else
                {
                    props.DrawClosedGate(v, false);
                }
            }
            else
            {
                DrawDownNoGate(i, j);
            }
        }
        else
        {
            DrawDownNoGate(i, j);
        }
    }

    private void DrawBlownUpPath(int i, int j, int size, Vector2Int v)
    {
        if (i - 1 > -1 && mapData.findMap[i - 1, j] && mapData.map[i - 1, j] == 0)
        {
            DrawLeftPath(i, j);
            DrawRightPath(i - 1, j);
        }
        if (i + 1 < size && mapData.findMap[i + 1, j] && mapData.map[i + 1, j] == 0)
        {
            DrawRightPath(i, j);
            DrawLeftPath(i + 1, j);
        }
        if (j + 1 < size && mapData.findMap[i, j + 1])
        {
            if (mapData.map[i, j + 1] == 0)
            {
                DrawUpPath(i, j);
                DrawDownPath(i, j + 1);
                DeleteUpNoGate(i, j);
                if (mapData.goneMap[i, j + 1] > 0)
                {
                    props.DrawOpenedGate(new Vector2Int(i * 12, (j + 1) * 12), false);
                }
                else
                {
                    props.DrawClosedGate(new Vector2Int(i * 12, (j + 1) * 12), false);
                }
            }
        }
        if (j - 1 > -1 && mapData.findMap[i, j - 1])
        {
            if (mapData.map[i, j - 1] == 0)
            {
                DrawDownPath(i, j);
                DrawUpPath(i, j - 1);
                DeleteDownNoGate(i, j);
                if (mapData.goneMap[i, j - 1] > 0)
                {
                    props.DrawOpenedGate(new Vector2Int(i * 12, (j - 1) * 12), false);
                }
                else
                {
                    props.DrawClosedGate(new Vector2Int(i * 12, (j - 1) * 12), false);
                }
            }
        }
    }

    private void DrawDark(int x, int y)
    {
        for (int i = 2; i < 10; i++)
        {
            for (int j = 3; j < 11; j++)
            {
                darkMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), dark[i, j]);
            }
        }
    }

    private void DeleteDark(int x, int y)
    {
        for (int i = 2; i < 10; i++)
        {
            for (int j = 3; j < 11; j++)
            {
                darkMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), dark[0, 0]);
            }
        }
    }

    private void DrawFrame(int x, int y)
    {
        for (int i = 1; i < 11; i++)
        {
            for (int j = 2; j < 12; j++)
            {
                frameMap.SetTile(new Vector3Int(x * 12 + i, y * 12 + j), frame[i, j]);
            }
        }
    }

    private IEnumerator FlickerFrame()
    {
        while (true)
        {
            for (float i = 1; i > 0; i -= 0.005f)
            {
                frameMap.color = new Color(1, 1, 1, i);
                yield return null;
            }
            for (float i = 0; i < 1; i += 0.005f)
            {
                frameMap.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }


    public void DeleteEvent()
    {
        props.DeleteEvent(mapData.position * 12);
    }

    public void DeleteIcon()
    {
        props.DeleteIcon(mapData.position * 12);
    }

    public void DrawChest(int chestType)
    {
        props.DrawChest(mapData.position * 12, chestType);
    }

    public void DrawIcon()
    {
        Vector2Int position = mapData.position;
        if (position.x -1 > -1 && mapData.knownMap[position.x - 1, position.y] && mapData.goneMap[position.x - 1, position.y] < 1)
        {
            props.DrawIcon(mapData.map[position.x - 1, position.y], new Vector2Int(position.x-1, position.y) * 12);
        }
        if (position.x + 1 < GameManager.Inst.Exploration.map.MapSize && mapData.knownMap[position.x + 1, position.y] && mapData.goneMap[position.x + 1, position.y] < 1)
        {
            props.DrawIcon(mapData.map[position.x + 1, position.y], new Vector2Int(position.x + 1, position.y) * 12);
        }
        if (position.y - 1 > -1 && mapData.knownMap[position.x, position.y - 1] && mapData.goneMap[position.x, position.y - 1] < 1)
        {
            props.DrawIcon(mapData.map[position.x, position.y - 1], new Vector2Int(position.x, position.y-1) * 12);
        }
        if (position.y + 1 < GameManager.Inst.Exploration.map.MapSize && mapData.knownMap[position.x, position.y + 1] && mapData.goneMap[position.x, position.y + 1] < 1)
        {
            props.DrawIcon(mapData.map[position.x, position.y + 1], new Vector2Int(position.x, position.y+1) * 12);
        }
    }


    public void DrawMap()
    {
        int size = GameManager.Inst.Exploration.map.MapSize;

        frameMap.ClearAllTiles();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (mapData.findMap[i, j])
                {
                    Vector2Int v = new Vector2Int(i * 12, j * 12);
                    
                    if (mapData.map[i, j] > 0 && mapData.map[i, j] < 13)
                    {
                        DrawRoom(i, j);

                        DrawPathes(i, j, size, v);

                        if (mapData.blownupMap[i, j])
                        {
                            DrawBlownUpPath(i, j, size, v);
                        }

                        if (mapData.goneMap[i,j] > 0)
                        {
                            DeleteDark(i, j); 
                            props.DeleteIcon(new Vector2Int(i, j) * 12);
                        }
                        else
                        {
                            DrawDark(i, j);
                            if (mapData.knownMap[i, j])
                                props.DrawIcon(mapData.map[i, j], new Vector2Int(i, j) * 12);
                        }

                        if ((Mathf.Abs(mapData.position.x - i) == 1 && mapData.position.y == j)
                            || (Mathf.Abs(mapData.position.y - j) == 1 && mapData.position.x == i))
                        {
                            DrawFrame(i, j);
                        }

                        if (mapData.map[i,j] == 12)
                        {
                            mapManager.DrawMerchant(new Vector2Int(i, j) * 12);
                        }
                    }
                    else if (mapData.map[i,j] == (int)TileType.Secret)
                    {
                        DrawRoom(i, j);

                        if (mapData.goneMap[i, j] > 0)
                        {
                            props.DrawOpenedGate(v, true);
                            props.DrawOpenedGate(v, false);
                            DeleteDark(i, j);
                            DeleteIcon();
                        }
                        else
                        {
                            props.DrawClosedGate(v, true);
                            props.DrawClosedGate(v, false);
                            DrawDark(i, j);
                            props.DrawIcon(mapData.map[i, j], new Vector2Int(i, j) * 12);
                        }
                    }
                }
            }
        }
    }

    private void ClearTilemaps()
    {
        frameMap.ClearAllTiles();
        darkMap.ClearAllTiles();
        wall3Map.ClearAllTiles();
        wall2Map.ClearAllTiles();
        wall1Map.ClearAllTiles();
        shadowMap.ClearAllTiles();
        groundMap.ClearAllTiles();
    }

    private void Awake()
    {
        if (!TryGetComponent<MapManager>(out mapManager))
        {
            Debug.Log("MapPainter - Awake - MapManager");
        }
        if (!GameObject.Find("MapProps").TryGetComponent<MapProps>(out props))
        {
            Debug.Log("Map Creater - Awake - MapProps");
        }

        frame = new TileBase[12, 12];
        dark = new TileBase[12, 12];
        wall3 = new TileBase[12, 12];
        wall2 = new TileBase[12, 12];
        wall1 = new TileBase[12, 12];
        shadow = new TileBase[12, 12];
        ground = new TileBase[12, 12];

        if (transform.childCount == 7)
        {
            if (transform.GetChild(0).TryGetComponent<Tilemap>(out frameMap)
                && transform.GetChild(1).TryGetComponent<Tilemap>(out darkMap)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out wall3Map)
                && transform.GetChild(3).TryGetComponent<Tilemap>(out wall2Map)
                && transform.GetChild(4).TryGetComponent<Tilemap>(out wall1Map)
                && transform.GetChild(5).TryGetComponent<Tilemap>(out shadowMap)
                && transform.GetChild(6).TryGetComponent<Tilemap>(out groundMap))
            {
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        frame[i, j] = frameMap.GetTile(new Vector3Int(i, j));
                        dark[i, j] = darkMap.GetTile(new Vector3Int(i, j));
                        wall3[i, j] = wall3Map.GetTile(new Vector3Int(i, j));
                        wall2[i, j] = wall2Map.GetTile(new Vector3Int(i, j));
                        wall1[i, j] = wall1Map.GetTile(new Vector3Int(i, j));
                        shadow[i, j] = shadowMap.GetTile(new Vector3Int(i, j));
                        ground[i, j] = groundMap.GetTile(new Vector3Int(i, j));
                    }
                }
            }

            ClearTilemaps();
        }
    }

    private void Start()
    {
        StartCoroutine(FlickerFrame());
    }

}
