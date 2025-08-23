using UnityEngine;

public interface ICapability { }
public class BlockMask : ICapability { public bool[,] Mask { get; set; } }
public class TileLayer : ICapability { public TileTypeSO?[,] Tiles { get; set; } }
public class IntLayer : ICapability { public string Name { get; set; } public int[,] Data { get; set; } }
public class FloatLayer : ICapability { public string Name { get; set; } public float[,] Data { get; set; } }
public class BoolLayer : ICapability { public string Name { get; set; } public bool[,] Data { get; set; } }
public class Dimensions : ICapability { public Vector2Int MapDimensions { get; set; } }