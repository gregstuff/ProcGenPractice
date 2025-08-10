using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.SO;

namespace DungeonGeneration.Map.Gen
{
    public interface IDungeonLevelGenerator
    {
        public ILevel GenerateDungeonLevel(GeneratedLevelLayoutSO levelLayout);
    }
}
