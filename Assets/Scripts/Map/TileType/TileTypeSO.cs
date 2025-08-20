using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Tile Type")]
public class TileTypeSO : ScriptableObject
{
    public string displayName;
    public TileTypeTags[] tags; 

    public bool HasTag(string tag) =>
        tags != null && Array.IndexOf(tags, tag) >= 0;
}
