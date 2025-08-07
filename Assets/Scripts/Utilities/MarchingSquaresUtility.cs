using System;

public static class MarchingSquaresUtility
{
    public static int[,] ToMarchingSquaresInts(this bool[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);

        int[,] result = new int[height - 1, width - 1];

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int caseValue = 0;

                if (grid[y + 1, x]) caseValue |= 1;

                if (grid[y + 1, x + 1]) caseValue |= 2;

                if (grid[y, x]) caseValue |= 4;

                if (grid[y, x + 1]) caseValue |= 8;

                result[y, x] = caseValue;
            }
        }

        return result;
    }
}