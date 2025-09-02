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
        private string _cachedKey;

        public bool HasTag(TileTag tag) =>
            tags != null && Array.IndexOf(tags, tag) >= 0;

        public string Key => GetKey();

        private string GetKey()
        {
            if (_cachedKey == null) return _cachedKey;

            _cachedKey = tags != null ? string.Join("|", tags) : string.Empty;

            return _cachedKey;
        }
    }
}

