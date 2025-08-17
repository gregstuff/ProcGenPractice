using Codice.CM.WorkspaceServer.Tree;
using DungeonGeneration.Map.Enum;
using System;
using UnityEngine;

[Serializable]
public class DecorationRule
{
    public string Name;
    public TileType[,] MatchingPattern;
    public Vector2 SpawnCell;
    public Vector2[] PostSpawnBlockedCells;
    public GameObject Prefab;
    public Vector3 SpawnScale;
    public Vector3 SpawnRotation;
    public Vector3 SpawnPositionOffset;
    public int MaxApplications;

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
