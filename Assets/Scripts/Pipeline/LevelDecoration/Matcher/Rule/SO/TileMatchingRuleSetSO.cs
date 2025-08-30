using System;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [CreateAssetMenu(menuName = "ProcGen/Tile/Matcher/Tile Matcher Rule Set")]
    [Serializable]
    public class TileMatchingRuleSetSO : ScriptableObject
    {
        [SerializeField] public TileMatchingRuleSO[] TileMatchingRules;
        [SerializeField] public TileMatchingRuleSO DefaultMatchingRule;
    }
}