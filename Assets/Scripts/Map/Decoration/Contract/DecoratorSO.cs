
using UnityEngine;

public abstract class DecoratorSO : ScriptableObject
{
    public abstract void ApplyDecorations(ICapabilityProvider level);
}