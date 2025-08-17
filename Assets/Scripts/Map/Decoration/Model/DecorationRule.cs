using Codice.CM.WorkspaceServer.Tree;
using DungeonGeneration.Map.Enum;
using System;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    public string Name { get; set; }
    public TileType[,] MatchingPattern { get; set; }
    public Vector2 SpawnCell { get; set; }
    public Vector2[] PostSpawnBlockedCells { get; set; }
    public GameObject Prefab { get; set; }
    public Vector3 SpawnScale { get; set; }
    public Vector3 SpawnRotation { get; set; }
    public Vector3 SpawnPositionOffset { get; set; }
    public int MaxApplications { get; set; }

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
        matchingPattern = MatchingPattern;
        spawnCell = SpawnCell;
        postSpawnBlockedCells = PostSpawnBlockedCells;
        prefab = Prefab;
        spawnScale = SpawnScale;
        spawnRotation = SpawnRotation;
        spawnPositionOffset = SpawnPositionOffset;
    }

}
