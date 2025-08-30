
using ProcGenSys.Common.LevelBundle;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDataGeneration.Generators
{
    public abstract class LevelGeneratorSO : ScriptableObject
    {
        public abstract ICapabilityProvider GenerateLevel();
    }
}