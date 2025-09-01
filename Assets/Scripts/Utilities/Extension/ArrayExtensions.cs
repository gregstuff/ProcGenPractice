
using ProcGenSys.Common.Enum;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Serialization.NodeTypeResolvers;

public static class ArrayExtensions
{
    public static IEnumerable<(Direction, T)> GetAdjacentNeighbors<T>(this T[,] array, int row, int col)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        if (row > 0)
            yield return (Direction.North, array[row - 1, col]);
        if (col < cols - 1)
            yield return (Direction.East, array[row, col + 1]);
        if (row < rows - 1)
            yield return (Direction.South, array[row + 1, col]);
        if (col > 0)
            yield return (Direction.West, array[row, col - 1]);
    }

    public static IEnumerable<(Direction, T)> GetAdjacentNeighborsWithDiagonals<T>(this T[,] array, int row, int col)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        foreach (var n in array.GetAdjacentNeighbors(row, col))
            yield return n;

        if (row > 0 && col < cols - 1)
            yield return (Direction.NorthEast, array[row - 1, col + 1]);
        if (row < rows - 1 && col < cols - 1)
            yield return (Direction.SouthEast, array[row + 1, col + 1]);
        if (row < rows - 1 && col > 0)
            yield return (Direction.SouthWest, array[row + 1, col - 1]);
        if (row > 0 && col > 0)
            yield return (Direction.NorthWest, array[row - 1, col - 1]);

    }

    public static IEnumerable<(Direction, T)> Get2x2<T>(this T[,] array, int row, int col)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        yield return (Direction.None, array[row, col]);
        yield return (row < rows - 1) ? (Direction.South, array[row + 1, col]) : default;
        yield return (col < cols - 1) ? (Direction.East, array[row, col + 1]) : default;
        yield return (row < rows - 1 && col < cols - 1) ? (Direction.SouthEast, array[row + 1, col + 1]) : default;
    }

}
