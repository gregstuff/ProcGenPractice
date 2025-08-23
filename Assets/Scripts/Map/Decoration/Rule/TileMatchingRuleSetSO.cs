using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Tile/Matcher/Tile Matcher Rule Set")]
[Serializable]
public class TileMatchingRuleSetSO : ScriptableObject
{
    [SerializeField] public TileMatchingRuleSO[] TileMatchingRules;
}