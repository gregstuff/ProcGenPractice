using System;

[Serializable]
public class BlockedMatchingRule : IMatchingRule
{
    public bool MatchBlocked;
    public bool Matches(TileTypeSO tileType) { return (tileType.blocking && MatchBlocked) || (!tileType.blocking && !MatchBlocked); }
}
