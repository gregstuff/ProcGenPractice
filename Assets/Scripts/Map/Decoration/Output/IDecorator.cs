using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.Output.SO;

public interface IDecorator
{
    void ApplyDecorations(ILevel level, DungeonOutputConfigSO tileset);
}