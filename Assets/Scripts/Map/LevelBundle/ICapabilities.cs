public interface ICapability { }
public interface IBlockMask : ICapability { bool[,] Mask { get; } }
public interface ITileLayer : ICapability { TileTypeSO?[,] Tiles { get; } }
public interface IIntLayer : ICapability { string Name { get; } int[,] Data { get; } }
public interface IFloatLayer : ICapability { string Name { get; } float[,] Data { get; } }
public interface IBoolLayer : ICapability { string Name { get; } bool[,] Data { get; } }