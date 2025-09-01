using System;
using System.Linq;
using UnityEngine;

namespace ProcGenSys.Common.Tile
{
    [Serializable]
    [CreateAssetMenu(menuName = "ProcGen/Tile/Type/Tile Type")]
    public class TileTypeSO : ScriptableObject
    {
        public string displayName;
        public TileTag[] tags;
        public bool blocking;

        public bool HasTag(TileTag tag) =>
            tags != null && Array.IndexOf(tags, tag) >= 0;

        public string Key => tags != null ? string.Join("|", tags) : string.Empty;
    }
}

