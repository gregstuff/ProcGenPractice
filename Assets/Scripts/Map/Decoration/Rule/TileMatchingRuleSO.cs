using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Tile/Matcher/Tile Matcher")]
[Serializable]
public class TileMatchingRuleSO : ScriptableObject
{
    [SerializeField] public TileTag[] MatchingTags;
    public bool MatchesTile(TileTypeSO tile)
    {
        return tile != null && tile.tags.Intersect(MatchingTags).Any();
    }
}