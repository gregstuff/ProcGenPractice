using ProcGenSys.Common.Tile;
using System;
using System.Linq;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [Serializable]
    public class AllTileTagsEqualMatchingRule : IMatchingRule
    {
        public TileTag[] TileTags;
        public bool Matches(TileTypeSO tileType) { return TileTags.SequenceEqual(tileType.tags); }
    }
}