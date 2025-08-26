
using UnityEngine;

public abstract class LevelGeneratorSO : ScriptableObject
{
    public abstract ICapabilityProvider GenerateLevel();
}