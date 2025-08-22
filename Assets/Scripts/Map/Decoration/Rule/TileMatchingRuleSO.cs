using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileMatchingRuleSO : ScriptableObject
{
    [SerializeField] public String Name;
    [SerializeField] public TileTag[] MatchingTags;

    public bool MatchesTile(TileTypeSO tile)
    {
        return tile != null && tile.tags.Intersect(MatchingTags).Any();
    }

}