using ProcGenSys.Common.LevelBundle;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher
{
    public abstract class DecorationMatcherSO : ScriptableObject
    {
        public abstract IEnumerable<IDecorationMatch> GetDecorationMatches(ICapabilityProvider level);
    }
}
