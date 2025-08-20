using DungeonGeneration.Map.SO;

namespace DungeonGeneration.Map.Gen
{
    public interface IDungeonLevelGenerator
    {
        public ICapabilityProvider GenerateDungeonLevel(GeneratedLevelLayoutSO levelLayout);
    }
}
