
using System.Collections.Generic;
using UnityEngine;

namespace DungeonGeneration.Utilities
{
    public class GridUtility
    {
        public static List<Vector2Int> SetLineForGrid<T>(T[,] grid, T val, Vector2Int p0, Vector2Int p1)
        {
            var points = new List<Vector2Int>();

            int dx = Mathf.Abs(p1.x - p0.x);
            int dy = Mathf.Abs(p1.y - p0.y);
            int sx = (p0.x < p1.x) ? 1 : -1;
            int sy = (p0.y < p1.y) ? 1 : -1;
            int err = dx - dy;

            while (p0 != p1)
            {
                grid[p0.y, p0.x] = val;

                int e2 = 2 * err;

                if (e2 > -dy && e2 < dx)
                {
                    grid[p0.x + sx, p0.y] = val;
                    points.Add(new Vector2Int(p0.x + sx, p0.y));
                }

                if (e2 > -dy)
                {
                    err -= dy;
                    p0.x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    p0.y += sy;
                }
            }
            return points;
        }

        public static List<Vector2Int> SetRectForGrid<T>(T[,] grid, T val, RectInt rect)
        {
            var points = new List<Vector2Int>();
            for (int y = rect.yMin; y < rect.yMax; y++)
            {
                for (int x = rect.xMin; x < rect.xMax; x++)
                {
                    grid[y, x] = val;
                    points.Add(new Vector2Int(x, y));
                }
            }
            return points;
        }

        public static List<Vector2Int> SetTextureForGrid<T>(T[,] grid, T val, Texture2D sourceTexture, RectInt destinationRect)
        {
            var points = new List<Vector2Int>();
            int gridWidth = grid.GetLength(0);
            int gridHeight = grid.GetLength(1);

            int startX = Mathf.Max(0, destinationRect.x);
            int startY = Mathf.Max(0, destinationRect.y);
            int endX = Mathf.Min(destinationRect.x + destinationRect.width, gridWidth);
            int endY = Mathf.Min(destinationRect.y + destinationRect.height, gridHeight);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    Color sourceColor = sourceTexture.GetPixel(x - destinationRect.x, y - destinationRect.y);

                    if (sourceColor == Color.white)
                    {
                        grid[y, x] = val; 
                        points.Add(new Vector2Int(x,y));
                    }
                }
            }
            return points;
        }
    }
}