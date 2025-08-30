
using ProcGenSys.Common.LevelBundle;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelGeometryGeneration
{
    public abstract class OutputGeneratorSO : ScriptableObject
    {
        public abstract void OutputMap(ICapabilityProvider level);
    }
}
