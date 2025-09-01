using UnityEngine;
using ProcGenSys.Common.Tile;

namespace ProcGenSys.WFC.Marker
{
    public class WFCLevelExemplar : MonoBehaviour
    {
        [SerializeField] private Vector2Int mapDimensions;
        [SerializeField] private float cellSize = 1f;

        [SerializeField] private TileTypeSO[] tilesFlat;

        public void SetGrid(Vector2Int dims, float size, TileTypeSO[,] tiles2D)
        {
            if (dims.x <= 0 || dims.y <= 0)
                throw new System.ArgumentException("Dimensions must be > 0");

            if (tiles2D == null || tiles2D.GetLength(0) != dims.y || tiles2D.GetLength(1) != dims.x)
                throw new System.ArgumentException("tiles2D shape does not match dims");

            mapDimensions = dims;
            cellSize = size;

            tilesFlat = new TileTypeSO[dims.x * dims.y];
            for (int r = 0; r < dims.y; r++)
                for (int c = 0; c < dims.x; c++)
                    tilesFlat[r * dims.x + c] = tiles2D[r, c];
        }

        public bool TryGetGrid(out TileTypeSO[,] tiles2D, out int rows, out int cols, out float outCellSize)
        {
            cols = mapDimensions.x;
            rows = mapDimensions.y;
            outCellSize = cellSize;

            if (cols <= 0 || rows <= 0 || tilesFlat == null || tilesFlat.Length != rows * cols)
            {
                tiles2D = null;
                return false;
            }

            tiles2D = new TileTypeSO[rows, cols];
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    tiles2D[r, c] = tilesFlat[r * cols + c];

            return true;
        }
    }
}
