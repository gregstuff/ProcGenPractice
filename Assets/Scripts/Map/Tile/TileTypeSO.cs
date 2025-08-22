using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Tile Type")]
public class TileTypeSO : ScriptableObject
{
    public string displayName;
    public TileTag[] tags;
    public bool blocking;

    public bool HasTag(TileTag tag) =>
        tags != null && Array.IndexOf(tags, tag) >= 0;
}
