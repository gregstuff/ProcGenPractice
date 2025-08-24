using System;

[Serializable]
public class NullMatchingRule : IMatchingRule
{
    public bool MatchNull;
    public bool Matches(TileTypeSO tileType) { return (tileType == null && MatchNull) || (tileType != null && !MatchNull); }
}