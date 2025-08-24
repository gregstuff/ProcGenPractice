
using UnityEngine;

namespace DungeonGeneration.Map.Output
{
    public abstract class OutputGeneratorSO : ScriptableObject
    {
        public abstract void OutputMap(ICapabilityProvider level);
    }
}