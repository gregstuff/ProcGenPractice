using ProcGenSys.Common.Tile;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSys.Common.LevelBundle
{
    public interface ICapability { }
    public class BlockMask : ICapability { public bool[,] Mask { get; set; } }
    public class TileLayer : ICapability { public TileTypeSO?[,] Tiles { get; set; } }
    public class IntLayer : ICapability { public string Name { get; set; } public int[,] Data { get; set; } }
    public class FloatLayer : ICapability { public string Name { get; set; } public float[,] Data { get; set; } }
    public class BoolLayer : ICapability { public string Name { get; set; } public bool[,] Data { get; set; } }
    public class Dimensions : ICapability { public Vector2Int MapDimensions { get; set; } }
    public class Scale : ICapability { public float MapScale { get; set; } }
    public class SpaceMap : ICapability { public Dictionary<Vector2Int, string> Map { get; set; } }
}
