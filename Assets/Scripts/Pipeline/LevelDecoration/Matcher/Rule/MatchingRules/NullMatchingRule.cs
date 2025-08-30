using ProcGenSys.Common.Tile;
using System;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [Serializable]
    public class NullMatchingRule : IMatchingRule
    {
        public bool MatchNull;
        public bool Matches(TileTypeSO tileType) { return (tileType == null && MatchNull) || (tileType != null && !MatchNull); }
    }
}