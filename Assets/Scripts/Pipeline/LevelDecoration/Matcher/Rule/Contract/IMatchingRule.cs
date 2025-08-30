
using ProcGenSys.Common.Tile;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    public interface IMatchingRule
    {
        bool Matches(TileTypeSO tileType);
    }
}