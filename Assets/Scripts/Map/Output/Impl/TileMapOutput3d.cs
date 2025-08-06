
using System.Text;
using UnityEngine;

public class TileMapOutput3d : IDungeonOutput
{
    public void OutputMap(Level level)
    {
        var grid = level.ToBlockedMap();
        var mappedMarchingSquareGrid = MarchingSquaresUtility.ToMarchingSquaresInts(grid);

        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < mappedMarchingSquareGrid.GetLength(0); ++y)
        {
            for (int x = 0; x < mappedMarchingSquareGrid.GetLength(1); ++x)
            {
                sb.Append(mappedMarchingSquareGrid[y, x].ToString("D2"));
                sb.Append(" ");
            }
        }

        Debug.Log(sb.ToString());

    }
}
