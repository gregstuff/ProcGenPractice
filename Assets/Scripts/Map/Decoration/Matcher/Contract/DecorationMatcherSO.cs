using System.Collections.Generic;
using UnityEngine;

public abstract class DecorationMatcherSO : ScriptableObject
{
    public abstract IEnumerable<IDecorationMatch> GetDecorationMatches(ICapabilityProvider level);
}
