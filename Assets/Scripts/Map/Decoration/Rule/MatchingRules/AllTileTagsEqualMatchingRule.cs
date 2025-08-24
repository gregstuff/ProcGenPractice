using System.Linq;
using System;

[Serializable]
public class AllTileTagsEqualMatchingRule : IMatchingRule
{
    public TileTag[] TileTags;
    public bool Matches(TileTypeSO tileType) { return TileTags.SequenceEqual(tileType.tags); }
}