using DungeonGeneration.Map.Enum;
using UnityEngine;

[System.Serializable]
public class DecorationRuleUIModel
{
    public string Name { get; set; } = "Rule_" + System.Guid.NewGuid().ToString().Substring(0, 4);
    private int _width = 3;
    private int _height = 3;
    private TileType[,] _pattern;

    public Vector2 SpawnCell { get; set; }
    public Vector2[] PostSpawnBlockedCells { get; set; }
    [SerializeField] public GameObject Prefab { get; set; }
    public Vector3 SpawnScale { get; set; }
    public Vector3 SpawnRotation { get; set; }
    public Vector3 SpawnPositionOffset { get; set; }
    public int MaxApplications { get; set; } 
    public TileType[,] MatchingPattern => _pattern;

    public DecorationRuleUIModel()
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
        gridID = Name;
    }

}