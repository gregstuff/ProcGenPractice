using ProcGenSys.Common.Tile;
using System;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [Serializable]
    public class TileTagContainsMatchingRule : IMatchingRule
    {
        public TileTag tag;
        public bool Matches(TileTypeSO tileType) { return tileType.HasTag(tag); }
    }
}