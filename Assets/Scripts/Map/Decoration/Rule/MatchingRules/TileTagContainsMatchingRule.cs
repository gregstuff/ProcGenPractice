using System;

[Serializable]
public class TileTagContainsMatchingRule : IMatchingRule
{
    public TileTag tag;
    public bool Matches(TileTypeSO tileType) { return tileType.HasTag(tag); }
}