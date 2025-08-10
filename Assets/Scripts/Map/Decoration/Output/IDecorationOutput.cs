using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.SO;
using static DecorationPatternMatcher;
using System.Collections.Generic;
using UnityEngine;

public interface IDecorationOutput
{
    void ApplyDecorations(List<DecorationPlacement> placements, Transform parent, ILevel level, TilesetConfigSO tileset);
}