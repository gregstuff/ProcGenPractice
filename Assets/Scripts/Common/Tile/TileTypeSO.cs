using System;
using UnityEngine;

namespace ProcGenSys.Common.Tile
{
    [CreateAssetMenu(menuName = "ProcGen/Tile/Type/Tile Type")]
    public class TileTypeSO : ScriptableObject
    {
        public string displayName;
        public TileTag[] tags;
        public bool blocking;

        public bool HasTag(TileTag tag) =>
            tags != null && Array.IndexOf(tags, tag) >= 0;
    }
}

