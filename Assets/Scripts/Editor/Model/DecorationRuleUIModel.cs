using DungeonGeneration.Map.Enum;
using System;
using UnityEngine;

[System.Serializable]
public class DecorationRuleUIModel
{
    public string Name { get; set; } = "Rule_" + System.Guid.NewGuid().ToString().Substring(0, 4);
    private int _width = 3;
    private int _height = 3;
    private TileType[,] _matchingPattern;
    private bool[,] _spawningLocationGrid;
    private bool[,] _blockingLocationGrid;

    public Vector2 SpawnCell { get; set; }
    public Vector2[] PostSpawnBlockedCells { get; set; }
    [SerializeField] public GameObject Prefab { get; set; }
    public Vector3 SpawnScale { get; set; }
    public Vector3 SpawnRotation { get; set; }
    public Vector3 SpawnPositionOffset { get; set; }
    public int MaxApplications { get; set; } 
    public TileType[,] MatchingPattern => _matchingPattern;

    public DecorationRuleUIModel()
    {
        _matchingPattern = new TileType[_height, _width];
        _spawningLocationGrid = new bool[_height, _width];
        _blockingLocationGrid = new bool[_height, _width];
    }

    public void ResizeGrid(int newHeight, int newWidth)
    {

        _height = Mathf.Clamp(newHeight,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        _width = Mathf.Clamp(newWidth,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        _matchingPattern = new TileType[newHeight, newWidth];
        _spawningLocationGrid = new bool[newHeight, newWidth];
        _blockingLocationGrid = new bool[newHeight, newWidth];
    }

    public void Deconstruct(
        out string name,
        out int gridWidth,
        out int gridHeight,
        out TileType[,] gridPattern,
        out bool[,] spawnLocationGrid,
        out bool[,] blockLocationGrid
        )
    {
        name = Name;
        gridWidth = _width;
        gridHeight = _height;
        gridPattern = _matchingPattern;
        spawnLocationGrid = _spawningLocationGrid;
        blockLocationGrid = _blockingLocationGrid;
    }

    public static DecorationRuleUIModel FromModel(DecorationRule rule)
    {
        return new DecorationRuleUIModel()
        {
            _height = rule.MatchingPattern.GetLength(0),
            _width = rule.MatchingPattern.GetLength(1),
            _matchingPattern = rule.MatchingPattern,
            SpawnCell = rule.SpawnCell,
            SpawnScale = rule.SpawnScale,
            SpawnRotation = rule.SpawnRotation,
            SpawnPositionOffset = rule.SpawnPositionOffset,
            MaxApplications = rule.MaxApplications,
            PostSpawnBlockedCells = rule.PostSpawnBlockedCells,
            Prefab = rule.Prefab,
        };
    }



}