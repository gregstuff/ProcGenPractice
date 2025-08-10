
using DungeonGeneration.Map.Model;

namespace DungeonGeneration.Map.Output
{
    public interface IDungeonOutput
    {
        public void OutputMap(ILevel level);
    }
}