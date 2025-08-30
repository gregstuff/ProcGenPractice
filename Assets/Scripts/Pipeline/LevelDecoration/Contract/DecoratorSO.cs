
using ProcGenSys.Common.LevelBundle;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration
{
    public abstract class DecoratorSO : ScriptableObject
    {
        public abstract void ApplyDecorations(ICapabilityProvider level);
    }
}