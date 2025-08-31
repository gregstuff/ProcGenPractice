using ProcGenSys.Common.LevelBundle;
using ProcGenSys.Common.Tile;
using UnityEngine;

namespace ProcGenSys.WFC.Marker
{
    public class WFCLevelExemplar : MonoBehaviour
    {
        public ICapabilityProvider Level;

        public bool TryGetGrid(out TileTypeSO[,] tiles, out int rows, out int cols, out float cellSize)
        {
            tiles = null; rows = cols = 0; cellSize = 1f;

            if (!Level.TryGet<Dimensions>(out var dimensions)
                || !Level.TryGet<Scale>(out var scale)
                || !Level.TryGet<TileLayer>(out var tileLayer))
                return false;

            cols = dimensions.MapDimensions.x;
            rows = dimensions.MapDimensions.y;
            cellSize = scale.MapScale;

            //copy cells to out var
            var src = tileLayer.Tiles;
            tiles = new TileTypeSO[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    tiles[r, c] = src[r, c];

            return true;
        }


    }
}