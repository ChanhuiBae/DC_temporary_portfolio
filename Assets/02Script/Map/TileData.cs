using UnityEngine;
using System.Collections.Generic;
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
public static class TileData
{
    public static int[] size = { 0, 6, 7, 8, 9, 10, 11 };
    public static int[] max = { 0, 16, 21, 28, 37, 48, 61 };
    public static int[] min = { 0, 13, 16, 21, 28, 37, 48 };

    public static int GetTileMin(int floor, int total, TileType type)
    {
        int quantity = 0;
        switch (floor)
        {
            case 1:
                if (total < 15)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 3;
                            break;
                        default:
                            quantity = 1;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 4;
                            break;
                        default:
                            quantity = 1;
                            break;
                    }
                }
                break;
            case 2:
                if (total < 19)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 4;
                            break;
                        case TileType.Chest:
                            quantity = 2;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 2;
                            break;
                        default:
                            quantity = 1;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 5;
                            break;
                        case TileType.Secret:
                            quantity = 1;
                            break;
                        default:
                            quantity = 2;
                            break;
                    }
                }
                break;
            case 3:
                if (total < 25)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 5;
                            break;
                        case TileType.Chest:
                            quantity = 3;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 3;
                            break;
                        default:
                            quantity = 2;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 6;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 2;
                            break;
                        case TileType.Secret:
                            quantity = 1;
                            break;
                        default:
                            quantity = 3;
                            break;
                    }
                }
                break;
            case 4:
                if (total < 33)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 7;
                            break;
                        case TileType.Chest:
                            quantity = 4;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 4;
                            break;
                        default:
                            quantity = 3;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 10;
                            break;
                        case TileType.Secret:
                            quantity = 3;
                            break;
                        default:
                            quantity = 4;
                            break;
                    }
                }
                break;
            case 5:
                if (total < 41)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 11;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 5;
                            break;
                        case TileType.Secret:
                            quantity = 4;
                            break;
                        default:
                            quantity = 6;
                            break;
                    }
                }
                else if (total < 45)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 13;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 5;
                            break;
                        case TileType.Secret:
                            quantity = 4;
                            break;
                        default:
                            quantity = 6;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 14;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 6;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 7;
                            break;
                    }
                }
                break;
            case 6:
                if (total < 53)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 17;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 8;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 7;
                            break;
                    }
                }
                else if (total < 58)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 18;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 7;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 8;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 19;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 8;
                            break;
                        case TileType.Secret:
                            quantity = 6;
                            break;
                        default:
                            quantity = 9;
                            break;
                    }
                }
                break;
        }
        return quantity;
    }

    public static int GetTileMax(int floor, int total, TileType type)
    {
        int quantity = 0;
        switch (floor)
        {
            case 1:
                if (total < 15)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 5;
                            break;
                        case TileType.Chest:
                            quantity = 2;
                            break;
                        default:
                            quantity = 1;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 6;
                            break;
                        case TileType.Chest:
                            quantity = 2;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 2;
                            break;
                        default:
                            quantity = 1;
                            break;
                    }
                }
                break;
            case 2:
                if (total < 19)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 6;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 1;
                            break;
                        case TileType.Chest:
                            quantity = 3;
                            break;
                        default:
                            quantity = 2;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 7;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 2;
                            break;
                        case TileType.Secret:
                            quantity = 2;
                            break;
                        default:
                            quantity = 3;
                            break;
                    }
                }
                break;
            case 3:
                if (total < 25)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 8;
                            break;
                        case TileType.Chest:
                            quantity = 4;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 3;
                            break;
                        default:
                            quantity = 2;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 9;
                            break;
                        case TileType.Chest:
                            quantity = 4;
                            break;
                        case TileType.Secret:
                            quantity = 2;
                            break;
                        default:
                            quantity = 3;
                            break;
                    }
                }
                break;
            case 4:
                if (total < 33)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 11;
                            break;
                        case TileType.Chest:
                            quantity = 5;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 4;
                            break;
                        default:
                            quantity = 3;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 14;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 4;
                            break;
                        case TileType.Secret:
                            quantity = 4;
                            break;
                        default:
                            quantity = 5;
                            break;
                    }
                }
                break;
            case 5:
                if (total < 41)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 15;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 5;
                            break;
                        case TileType.Chest:
                            quantity = 7;
                            break;
                        case TileType.PositiveEvent:
                            quantity = 6;
                            break;
                        case TileType.Secret:
                            quantity = 4;
                            break;
                    }
                }
                else if (total < 45)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 16;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 7;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 17;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 7;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 8;
                            break;
                    }
                }
                break;
            case 6:
                if (total < 53)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 21;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 7;
                            break;
                        case TileType.Secret:
                            quantity = 5;
                            break;
                        default:
                            quantity = 8;
                            break;
                    }
                }
                else if (total < 58)
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 22;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 8;
                            break;
                        case TileType.Secret:
                            quantity = 6;
                            break;
                        default:
                            quantity = 9;
                            break;
                    }
                }
                else
                {
                    switch (type)
                    {
                        case TileType.Monster:
                            quantity = 23;
                            break;
                        case TileType.MiddleBoss:
                            quantity = 9;
                            break;
                        case TileType.Secret:
                            quantity = 6;
                            break;
                        default:
                            quantity = 10;
                            break;
                    }
                }
                break;
        }
        return quantity;
    }
}