using ProcGenSys.Common.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSys.Common.LevelBundle
{
    public interface ICapability { }

    public class BlockMask : ICapability { public bool[,] Mask; }

    public class TileLayer : ICapability { public TileTypeSO?[,] Tiles; }

    public class IntLayer : ICapability { public string Name; public int[,] Data; }

    public class FloatLayer : ICapability { public string Name; public float[,] Data; }

    public class BoolLayer : ICapability { public string Name; public bool[,] Data; }

    public class Dimensions : ICapability { public Vector2Int MapDimensions; }

    public class Scale : ICapability { public float MapScale; }

    public class SpaceMap : ICapability { public Dictionary<Vector2Int, string> Map; }
}
