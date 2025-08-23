
using UnityEngine;

namespace DungeonGeneration.Map.Output
{
    public abstract class OutputGenerator : ScriptableObject
    {
        public abstract void OutputMap(ICapabilityProvider level);
    }
}