
using DungeonGeneration.Map.Enum;
using UnityEngine;

public class TilePalette
{

    public Color GetColorForTile(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.None:
                return Color.black;
            case TileType.Room:
                return Color.black;
            case TileType.Hallway:
                return Color.blue;
            case TileType.Door: 
                return Color.yellow;
            default: 
                return Color.black;
        }
    }


}
