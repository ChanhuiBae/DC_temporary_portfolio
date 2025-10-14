using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapProps : MonoBehaviour
{
    private Tilemap iconMap;
    private Tilemap campfireMap;
    private Tilemap propsMap;

    private TileBase[] icon;
    private TileBase[,] campfire;
    private TileBase[,] props;

    public void DrawClosedGate(Vector2Int point, bool h)
    {
        if(h) // up
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 9), props[5, 1]);
        else // down
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 1), props[5, 1]);
    }
    public void DrawOpenedGate(Vector2Int point, bool h)
    {
        if (h) // up
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 9), props[5, 9]);
        else // down
            propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 1), props[5, 9]);
    }

    public void DrawIcon(int tile, Vector2Int point)
    {
        switch ((TileType)tile)
        {
            case TileType.Boss:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[0]);
                break;
            case TileType.MiddleBoss:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[1]);
                break;
            case TileType.Monster:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[2]);
                break;
            case TileType.Chest:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[3]);
                break;
            case TileType.PositiveEvent:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[4]);
                break;
            case TileType.NegativeEvent:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[5]);
                break;
            case TileType.Secret:
                iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), icon[6]);
                break;
        }
    }

    public void DeleteIcon(Vector2Int point)
    {
        iconMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), campfire[0, 0]);
    }

    public void DrawCampfire(Vector2Int point)
    {
        campfireMap.SetTile(new Vector3Int(point.x + 5, point.y + 4), campfire[5, 4]);
        campfireMap.SetTile(new Vector3Int(point.x + 4, point.y + 7), campfire[4, 7]);
        campfireMap.SetTile(new Vector3Int(point.x + 7, point.y + 7), campfire[7, 7]);
    }

    public void EraseCampfire()
    {
        campfireMap.ClearAllTiles();
    }
    
    public void DrawChest(Vector2Int point, int index)
    {
        propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), props[index, 0]);
    }

    public void DrawMonster(Vector2Int point)
    {
        propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), props[1, 1]);
    }

    public void DeleteEvent(Vector2Int point)
    {
        propsMap.SetTile(new Vector3Int(point.x + 5, point.y + 5), props[0, 0]);
    }

    private void Awake()
    {
        icon = new TileBase[7];
        campfire = new TileBase[12, 12];
        props = new TileBase[12, 12];

        if (transform.childCount > 2)
        {
            if (transform.GetChild(0).TryGetComponent<Tilemap>(out iconMap)
                && transform.GetChild(1).TryGetComponent<Tilemap>(out campfireMap)
                && transform.GetChild(2).TryGetComponent<Tilemap>(out propsMap))
            {
                for (int i = 0; i < 12; i++)
                {
                    if(i < 7)
                    {
                        icon[i] = iconMap.GetTile(new Vector3Int(i, 0));
                    }
                    for (int j = 0; j < 12; j++)
                    {
                        campfire[i, j] = campfireMap.GetTile(new Vector3Int(i, j));
                        props[i, j] = propsMap.GetTile(new Vector3Int(i, j));
                    }
                }
            }
        }

        iconMap.ClearAllTiles();
        campfireMap.ClearAllTiles();
        propsMap.ClearAllTiles();
    }
}
