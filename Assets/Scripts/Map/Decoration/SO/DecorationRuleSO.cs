// Struct for a single cell in the pattern
using DungeonGeneration.Map.Enum;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public struct PatternCell
{
    public PatternCellType Type;
    public TileType TileType; // Only used if Type == SpecificTileType
}

// Base ScriptableObject for decoration rules
public abstract class DecorationRuleSO : ScriptableObject
{
    [Tooltip("2D grid defining the pattern to match. Rows are y, columns are x.")]
    public PatternCell[,] Pattern; // E.g., new PatternCell[height, width]

    [Tooltip("Higher priority rules take precedence on overlaps.")]
    public int Priority;

    [Tooltip("Probability (0-1) of applying this rule when matched.")]
    [Range(0f, 1f)]
    public float Probability = 1f;

    [Tooltip("Maximum instances of this rule per level.")]
    public int MaxInstances = int.MaxValue;

    [Tooltip("Prefabs to choose from (random selection if multiple).")]
    public List<GameObject> Prefabs;

    [Tooltip("Relative position within the pattern grid for spawn (e.g., (1,0) for middle top in 3x3).")]
    public Vector2Int RelativeSpawnPosition;

    [Tooltip("Additional offset in world space.")]
    public Vector3 Offset;

    [Tooltip("Rotation in degrees (multiples of 45 recommended).")]
    public float Rotation;

    [Tooltip("Directionality for placement (e.g., face a wall).")]
    public SpawnDirection Direction;

    // Method to get a random prefab
    public GameObject GetRandomPrefab()
    {
        if (Prefabs == null || Prefabs.Count == 0) return null;
        return Prefabs[UnityEngine.Random.Range(0, Prefabs.Count)];
    }
}