using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileMatchingRuleSetSO : ScriptableObject
{
    [SerializeField] public TileMatchingRuleSO[] TileMatchingRules;
}