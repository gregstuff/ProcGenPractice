using DungeonGeneration.Map.Enum;
using UnityEngine;

[System.Serializable]
public class GridRule
{
    public string _id = "Rule_" + System.Guid.NewGuid().ToString().Substring(0, 4);
    private int _width = 3;
    private int _height = 3;
    private TileType[,] _pattern;
    private bool _foldout = true;
    private int _maxApplications = -1;

    public GridRule()
    {
        _pattern = new TileType[_height, _width];
    }

    public void ResizeGrid(int newHeight, int newWidth)
    {

        _height = Mathf.Clamp(newHeight,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        _width = Mathf.Clamp(newWidth,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        _pattern = new TileType[newHeight, newWidth];
    }

    public void Deconstruct(out int gridWidth, out int gridHeight, out TileType[,] gridPattern, out string gridID)
    {
        gridWidth = _width;
        gridHeight = _height;
        gridPattern = _pattern;
        gridID = _id;
    }
}