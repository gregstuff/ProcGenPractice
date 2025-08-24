using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DecorationRuleUIModel
{
    public string Name { get; set; } = "Rule_" + System.Guid.NewGuid().ToString().Substring(0, 4);
    private int _width = 3;
    private int _height = 3;
    private TileMatchingRuleSO[,] _matchingPattern;
    private bool[,] _spawningLocationGrid;
    private bool[,] _blockingLocationGrid;

    public Vector2Int SpawnCell => GetSpawnCell();
    public Vector2Int[] PostSpawnBlockedCells => GetPostSpawnBlockedCells();
    [SerializeField] public GameObject Prefab { get; set; }
    public Vector3 SpawnScale { get; set; } = Vector3.one;
    public Vector3 SpawnRotation { get; set; }
    public Vector3 SpawnPositionOffset { get; set; }
    public int MaxApplications { get; set; } 
    public TileMatchingRuleSO[,] MatchingPattern => _matchingPattern;
    private TileMatchingRuleSO _defaultMatchingRule;

    public DecorationRuleUIModel (TileMatchingRuleSetSO tileMatchingRuleSet)
    {
        _spawningLocationGrid = new bool[_height, _width];
        _blockingLocationGrid = new bool[_height, _width];
        _defaultMatchingRule = tileMatchingRuleSet.DefaultMatchingRule;

        InitMatchingGrid();
    }

    public void ResizeGrid(int newHeight, int newWidth)
    {

        _height = Mathf.Clamp(newHeight,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        _width = Mathf.Clamp(newWidth,
            ProcGenRulesWindowConstants.MINIMUM_GRID_LENGTH,
            ProcGenRulesWindowConstants.MAXIMUM_GRID_LENGTH);

        InitMatchingGrid();
        _spawningLocationGrid = new bool[newHeight, newWidth];
        _blockingLocationGrid = new bool[newHeight, newWidth];
    }

    public void Deconstruct(
        out string name,
        out int gridWidth,
        out int gridHeight,
        out TileMatchingRuleSO[,] gridPattern,
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

    public static DecorationRuleUIModel FromModel(DecorationRule rule, TileMatchingRuleSetSO tileMatchingRuleSet)
    {
        var matchingPattern = rule.MatchingPattern2D;

        var uiModel = new DecorationRuleUIModel(tileMatchingRuleSet)
        {
            _height = matchingPattern.GetLength(0),
            _width = matchingPattern.GetLength(1),
            _matchingPattern = matchingPattern,
            SpawnScale = rule.SpawnScale,
            SpawnRotation = rule.SpawnRotation,
            SpawnPositionOffset = rule.SpawnPositionOffset,
            MaxApplications = rule.MaxApplications,
            Prefab = rule.Prefab,
        };

        uiModel._spawningLocationGrid = new bool[uiModel._height, uiModel._width];
        uiModel._blockingLocationGrid = new bool[uiModel._height, uiModel._width];
        uiModel.SetSpawnCell(rule.SpawnCell.y,rule.SpawnCell.x);
        new List<Vector2Int>(rule.PostSpawnBlockedCells).ForEach(cell => uiModel.SetBlockedCell(cell.x, cell.y, true));

        return uiModel;
    }

    public void SetSpawnCell(int posY, int posX)
    {
        for (int y=0;y<_spawningLocationGrid.GetLength(0);++y)
        {
            for (int x=0;x<_spawningLocationGrid.GetLength(1);++x)
            {
                _spawningLocationGrid[y, x] = false;
            }
        }
        _spawningLocationGrid[posY, posX] = true;
        SetBlockedCell(posY, posX, true);
    }

    public void SetBlockedCell(int y, int x, bool val)
    {
        _blockingLocationGrid[y, x] = val;
    }

    public Vector2Int[] GetPostSpawnBlockedCells()
    {
        var positions = new List<Vector2Int>();
        for (int y=0;y<_blockingLocationGrid.GetLength(0);++y)
        {
            for (int x=0;x<_blockingLocationGrid.GetLength(1);++x)
            {

            }
        }
        return positions.ToArray();
    }

    public Vector2Int GetSpawnCell()
    {
        for (int y=0;y<_spawningLocationGrid.GetLength(0);++y)
        {
            for (int x=0;x<_spawningLocationGrid.GetLength(1);++x)
            {
                if (_spawningLocationGrid[y, x]) return new Vector2Int(x, y);
            }
        }
        return Vector2Int.zero;
    }

    private void InitMatchingGrid()
    {
        _matchingPattern = new TileMatchingRuleSO[_height, _width];
        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _matchingPattern[y, x] = _defaultMatchingRule;
            }
        }
    }

}