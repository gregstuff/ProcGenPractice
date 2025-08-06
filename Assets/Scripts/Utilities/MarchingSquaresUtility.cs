using System;

public static class MarchingSquaresUtility
{
    public static int[,] ToMarchingSquaresInts(this bool[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int[,] result = new int[width - 1, height - 1];

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int caseValue = 0;

                if (grid[x, y]) caseValue |= 1;

                if (grid[x + 1, y]) caseValue |= 2;

                if (grid[x + 1, y + 1]) caseValue |= 4;

                if (grid[x, y + 1]) caseValue |= 8;

                result[x, y] = caseValue;
            }
        }

        return result;
    }
}