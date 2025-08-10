
// Class to match patterns and generate placements
using DungeonGeneration.Map.Enum;
using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.SO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecorationPatternMatcher
{

    // Struct for a matched decoration placement
    [Serializable]
    public struct DecorationPlacement
    {
        public DecorationRuleSO Rule;
        public Vector2Int MatchPosition; // Top-left corner of the pattern match on the map
        public Vector3 SpawnPosition;    // Computed 3D world position
        public Quaternion Rotation;      // Computed rotation
        public GameObject Prefab;        // Selected prefab
    }

    public List<DecorationPlacement> Match(ILevel level, List<DecorationRuleSO> rules, TilesetConfigSO tileset)
    {
        var placements = new List<DecorationPlacement>();

        // Sort rules by priority descending
        rules.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        // Track instances per rule
        var instanceCounts = new Dictionary<DecorationRuleSO, int>();
        foreach (var rule in rules)
        {
            instanceCounts[rule] = 0;
        }

        // Create a visited map to avoid overlapping placements
        bool[,] visited = new bool[level.Height, level.Width];

        for (int y = 0; y < level.Height; y++)
        {
            for (int x = 0; x < level.Width; x++)
            {
                if (visited[y, x]) continue;

                // Check each rule in priority order
                foreach (var rule in rules)
                {
                    if (instanceCounts[rule] >= rule.MaxInstances) continue;

                    if (MatchPatternAt(level, rule, x, y, visited))
                    {
                        // Apply probability
                        if (UnityEngine.Random.value > rule.Probability) continue;

                        // Compute spawn details
                        var placement = CreatePlacement(level, rule, x, y, tileset);

                        if (placement.Prefab != null)
                        {
                            placements.Add(placement);
                            instanceCounts[rule]++;
                            MarkVisited(visited, rule, x, y);
                            break; // Move to next position after applying highest priority
                        }
                    }
                }
            }
        }

        return placements;
    }

    private bool MatchPatternAt(ILevel level, DecorationRuleSO rule, int startX, int startY, bool[,] visited)
    {
        int patHeight = rule.Pattern.GetLength(0);
        int patWidth = rule.Pattern.GetLength(1);

        // Check bounds
        if (startX + patWidth > level.Width || startY + patHeight > level.Height) return false;

        // Check each cell
        for (int py = 0; py < patHeight; py++)
        {
            for (int px = 0; px < patWidth; px++)
            {
                var cell = rule.Pattern[py, px];
                int mapX = startX + px;
                int mapY = startY + py;

                if (visited[mapY, mapX]) return false; // Skip if already decorated

                bool isBlocked = level.GetBlockedMap()[mapY, mapX];
                TileType tileType = level.GetTileTypeAt(mapX, mapY);

                switch (cell.Type)
                {
                    case PatternCellType.Blocked:
                        if (!isBlocked) return false;
                        break;
                    case PatternCellType.Unblocked:
                        if (isBlocked) return false;
                        break;
                    case PatternCellType.SpecificTileType:
                        if (tileType != cell.TileType) return false;
                        break;
                    case PatternCellType.Wildcard:
                        // Matches anything
                        break;
                }
            }
        }

        return true;
    }

    private void MarkVisited(bool[,] visited, DecorationRuleSO rule, int startX, int startY)
    {
        int patHeight = rule.Pattern.GetLength(0);
        int patWidth = rule.Pattern.GetLength(1);

        for (int py = 0; py < patHeight; py++)
        {
            for (int px = 0; px < patWidth; px++)
            {
                visited[startY + py, startX + px] = true;
            }
        }
    }

    private DecorationPlacement CreatePlacement(ILevel level, DecorationRuleSO rule, int startX, int startY, TilesetConfigSO tileset)
    {
        // Compute 2D spawn position on grid
        int spawnX = startX + rule.RelativeSpawnPosition.x;
        int spawnY = startY + rule.RelativeSpawnPosition.y;

        // Convert to 3D world position (assume 1 unit per tile, adjust as needed)
        Vector3 spawnPos = new Vector3(spawnX, 0, spawnY) + rule.Offset;

        // Apply scale component-wise
        Vector3 scaledPos = new Vector3(
            spawnPos.x * tileset.tileScale.x,
            spawnPos.y * tileset.tileScale.y,
            spawnPos.z * tileset.tileScale.z
        );

        // Rotation as quaternion
        Quaternion rot = Quaternion.Euler(0, rule.Rotation, 0); // Assuming Y-axis rotation

        return new DecorationPlacement
        {
            Rule = rule,
            MatchPosition = new Vector2Int(startX, startY),
            SpawnPosition = scaledPos,
            Rotation = rot,
            Prefab = rule.GetRandomPrefab()
        };
    }

}