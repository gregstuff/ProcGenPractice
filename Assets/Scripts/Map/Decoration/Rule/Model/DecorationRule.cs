using System;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    public string Name;
    public TileMatchingRuleSO[] MatchingPattern1D;
    public Vector2Int SpawnCell;
    public Vector2Int[] PostSpawnBlockedCells;
    public GameObject Prefab;
    public Vector3 SpawnScale;
    public Vector3 SpawnRotation;
    public Vector3 SpawnPositionOffset;
    public int MaxApplications;
    public int PatternHeight;
    public int PatternWidth;

    public TileMatchingRuleSO[,] MatchingPattern2D => From1DTo2DArray();

    private TileMatchingRuleSO[,] _matchingPattern2D;

    public DecorationRule(DecorationRuleUIModel model)
    {
        Name = model.Name;
        SpawnCell = model.SpawnCell;
        PostSpawnBlockedCells = model.PostSpawnBlockedCells;
        Prefab = model.Prefab;
        SpawnScale = model.SpawnScale;
        SpawnRotation = model.SpawnRotation;
        SpawnPositionOffset = model.SpawnPositionOffset;
        MaxApplications = model.MaxApplications;

        SetMatchingPattern(model.MatchingPattern);
    }

    public void Deconstruct(
        out string name,
        out TileMatchingRuleSO[,] matchingPattern,
        out Vector2Int spawnCell,
        out Vector2Int[] postSpawnBlockedCells,
        out GameObject prefab,
        out Vector3 spawnScale,
        out Vector3 spawnRotation,
        out Vector3 spawnPositionOffset)
    {
        name = Name;
        matchingPattern = From1DTo2DArray();
        spawnCell = SpawnCell;
        postSpawnBlockedCells = PostSpawnBlockedCells;
        prefab = Prefab;
        spawnScale = SpawnScale;
        spawnRotation = SpawnRotation;
        spawnPositionOffset = SpawnPositionOffset;
    }

    public TileMatchingRuleSO[,] From1DTo2DArray()
    {
        if(_matchingPattern2D != null) return _matchingPattern2D;

        var result = new TileMatchingRuleSO[PatternHeight, PatternWidth];
        for (int i = 0; i < PatternHeight; i++)
        {
            for (int j = 0; j < PatternWidth; j++)
            {
                result[i, j] = MatchingPattern1D[i * PatternWidth + j];
            }
        }
        _matchingPattern2D = result;
        return result;
    }

    public void SetMatchingPattern(TileMatchingRuleSO[,] pattern)
    {

        PatternHeight = pattern.GetLength(0);
        PatternWidth = pattern.GetLength(1);
        MatchingPattern1D = new TileMatchingRuleSO[PatternHeight * PatternWidth];
        for (int i = 0; i < PatternHeight; i++)
        {
            for (int j = 0; j < PatternWidth; j++)
            {
                MatchingPattern1D[i * PatternWidth + j] = pattern[i, j];
            }
        }
    }

    public bool Matches(int y, int x, TileTypeSO tileType)
    {
        if (y < 0
            || x < 0
            || y >= _matchingPattern2D.GetLength(0)
            || x >= _matchingPattern2D.GetLength(1))
            throw new Exception($"Attempted to check out of range for decoration rule {Name} y: {y} x: {x}");

        return _matchingPattern2D[y, x].MatchesTile(tileType);
    }

}
