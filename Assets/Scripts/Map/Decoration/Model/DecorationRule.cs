using Codice.CM.WorkspaceServer.Tree;
using DungeonGeneration.Map.Enum;
using System;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    public string Name;
    public TileType[] MatchingPattern1D;
    public Vector2 SpawnCell;
    public Vector2[] PostSpawnBlockedCells;
    public GameObject Prefab;
    public Vector3 SpawnScale;
    public Vector3 SpawnRotation;
    public Vector3 SpawnPositionOffset;
    public int MaxApplications;
    public int PatternHeight;
    public int PatternWidth;

    public TileType[,] MatchingPattern2D => From1DTo2DArray();

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
        out TileType[,] matchingPattern,
        out Vector2 spawnCell,
        out Vector2[] postSpawnBlockedCells,
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

    public TileType[,] From1DTo2DArray()
    {
        var result = new TileType[PatternHeight, PatternWidth];
        for (int i = 0; i < PatternHeight; i++)
        {
            for (int j = 0; j < PatternWidth; j++)
            {
                result[i, j] = MatchingPattern1D[i * PatternWidth + j];
            }
        }
        return result;
    }

    public void SetMatchingPattern(TileType[,] pattern)
    {

        PatternHeight = pattern.GetLength(0);
        PatternWidth = pattern.GetLength(1);
        MatchingPattern1D = new TileType[PatternHeight * PatternWidth];
        for (int i = 0; i < PatternHeight; i++)
        {
            for (int j = 0; j < PatternWidth; j++)
            {
                MatchingPattern1D[i * PatternWidth + j] = pattern[i, j];
            }
        }
    }

}
