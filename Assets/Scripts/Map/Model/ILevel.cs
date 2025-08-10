using DungeonGeneration.Map.Enum;

namespace DungeonGeneration.Map.Model
{
    public interface ILevel
    {
        int Width { get; }
        int Height { get; }
        bool[,] GetBlockedMap();
        TileType GetTileTypeAt(int x, int y);
    }
}