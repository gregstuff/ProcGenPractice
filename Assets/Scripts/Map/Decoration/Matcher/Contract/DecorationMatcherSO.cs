using UnityEngine;

public abstract class DecorationMatcherSO : ScriptableObject
{
    public abstract IDecorationMatch[] GetDecorationMatchers(ICapabilityProvider level);
}
