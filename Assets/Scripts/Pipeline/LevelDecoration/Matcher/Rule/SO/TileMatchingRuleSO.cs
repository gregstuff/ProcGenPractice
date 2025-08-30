using ProcGenSys.Common.Tile;
using System;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    [CreateAssetMenu(menuName = "ProcGen/Tile/Matcher/Tile Matcher")]
    [Serializable]
    public class TileMatchingRuleSO : ScriptableObject
    {
        [SerializeField] public TileTag[] MatchingTags;
        [SerializeField] public bool Blocked;

        public MatchingRules kind = MatchingRules.Null;

        [SerializeReference] private IMatchingRule _choice;

        public Type CurrentChoiceType => _choice?.GetType();

        void OnValidate() => EnsureChoiceType();

        private void OnEnable()
        {
            EnsureChoiceType();
        }

        void EnsureChoiceType()
        {
            Type want = ResolveType(kind);
            if (_choice == null || _choice.GetType() != want)
            {
                _choice = (IMatchingRule)Activator.CreateInstance(want);
            }
        }

        public bool MatchesTile(TileTypeSO tile)
        {
            EnsureChoiceType();
            return _choice.Matches(tile);
        }

        private Type ResolveType(MatchingRules selectedRule)
        {
            switch (selectedRule)
            {
                case MatchingRules.Null:
                    return typeof(NullMatchingRule);
                case MatchingRules.Blocked:
                    return typeof(BlockedMatchingRule);
                case MatchingRules.AllTileTagsEqual:
                    return typeof(AllTileTagsEqualMatchingRule);
                case MatchingRules.TileTagsContain:
                    return typeof(TileTagContainsMatchingRule);
                default:
                    throw new ArgumentException($"Unkown Matching Rule!!! {selectedRule}");
            }
        }

    }
}