using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.SO;
using static DecorationPatternMatcher;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonLevelOutputBase : IDecorationOutput
{
    public abstract void OutputMap(ILevel level); // Existing

    public virtual void OutputMap(ILevel level, List<DecorationPlacement> placements, TilesetConfigSO tileset)
    {
        OutputMap(level); // First output the base map
        ApplyDecorations(placements, null, level, tileset); // Then decorations
    }

    public abstract void ApplyDecorations(List<DecorationPlacement> placements, Transform parent, ILevel level, TilesetConfigSO tileset);

    // Example for 3D output
    // public void ApplyDecorations(...) {
    //     foreach (var p in placements) {
    //         var adjustedPos = DecorationSpawnUtils.RaycastForPlacement(p.SpawnPosition, p.Rule.Direction);
    //         p.SpawnPosition = adjustedPos;
    //         DecorationSpawnUtils.SpawnDecoration(p, parent, tileset);
    //     }
    // }

    // Example for Debug output
    // public void ApplyDecorations(...) {
    //     foreach (var p in placements) {
    //         Debug.Log($"Decoration: {p.Rule.name} at {p.SpawnPosition}");
    //     }
    // }
}