using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Decorators/Matchers/Grid Decoration Matcher")]
public class GridDecorationMatcherSO : DecorationMatcherSO
{
    [SerializeField] private DecorationRulesetSO _decorationRuleset;

    private Dictionary<Vector2Int, List<DecorationRule>> _dimensionsToDecorationRules;

    private int _maxWidth;
    private int _maxHeight;

    private void Init()
    {
        _dimensionsToDecorationRules = new Dictionary<Vector2Int, List<DecorationRule>>();
        _maxWidth = 0;
        _maxHeight = 0;
    }

    public override IEnumerable<IDecorationMatch> GetDecorationMatches(ICapabilityProvider level)
    {
        Init();

        List<IDecorationMatch> decorationMatches = new List<IDecorationMatch>(); 

        if(!level.TryGet<TileLayer>(out var tileLayer))
        {
            throw new System.Exception($"Level Generated does not fufill {typeof(GridDecorationMatcherSO)}'s requirements");
        }

        TileTypeSO[,] tileRules = tileLayer.Tiles;

        InitPatternData();

        for (int y = 0; y < tileRules.GetLength(0); ++y)
        {
            for (int x = 0; x < tileRules.GetLength(1); ++x)
            {
                foreach (var matchingRule in getMatchingRulesForPos(y, x, tileRules))
                {
                    decorationMatches.Add(new DecorationMatch()
                    {
                        SpawnPosition = new Vector2Int(x,y),
                        ObjectToSpawn = matchingRule.Prefab,
                        Offset = matchingRule.SpawnPositionOffset,
                        Rotation = matchingRule.SpawnRotation,
                        Scale = matchingRule.SpawnScale,
                    });
                }
            }
        }

        return decorationMatches;
    }

    private IEnumerable<DecorationRule> getMatchingRulesForPos(int y, int x, TileTypeSO[,] tileRules)
    {
        var matchedRules = new List<DecorationRule>();

        //rectX and rectY define the boundaries of the rect we're checking for rules
        for (int rectY = y; rectY <= y + _maxHeight; ++rectY)
        {
            for (int rectX = x; rectX <= x + _maxWidth; ++rectX)
            {
                var width = rectX - x;
                var height = rectY - y;

                if (rectY + height >= tileRules.GetLength(0) 
                    || rectX + width >= tileRules.GetLength(1))
                    continue; //we have gone out of bounds. No match here, friends

                //get index for current rect and see if there's any rules associated with it
                Vector2Int dimensions = new Vector2Int(width, height);
                if (!_dimensionsToDecorationRules.TryGetValue(dimensions, out var rules))
                {
                    //no rules associated with this area
                    continue;
                }

                foreach (var rule in rules)
                {
                    bool failed = false;
                    for (int ruleY = 0; ruleY < dimensions.y; ++ruleY)
                    {
                        for (int ruleX = 0; ruleX < dimensions.x; ++ruleX)
                        {
                            if (!rule.Matches(ruleY, ruleX, tileRules[ruleY + y, ruleX + x]))
                            {
                                //if any position failed to match, the rule did not match
                                failed = true;
                                break;
                            }
                        }
                        if (failed) break;
                    }
                    if(!failed) matchedRules.Add(rule);
                }
            }
        }
        return matchedRules;
    }

    private void InitPatternData()
    {
        _maxWidth = int.MinValue;
        _maxHeight = int.MinValue;

        foreach (var decorationPatternRule in _decorationRuleset.DecorationRules)
        {
            var decorationPattern = decorationPatternRule.MatchingPattern2D;
            var height = decorationPattern.GetLength(0);
            var width = decorationPattern.GetLength(1);

            if(width > _maxWidth) _maxWidth = height;
            if(height > _maxHeight) _maxHeight = height;

            Vector2Int dimensions = new Vector2Int(width, height);

            if(!_dimensionsToDecorationRules.TryGetValue(dimensions, out var ruleList))
            {
                _dimensionsToDecorationRules[dimensions] = new List<DecorationRule>();
                ruleList = _dimensionsToDecorationRules[dimensions];
            }

            ruleList.Add(decorationPatternRule);
        }
    }

    private class DecorationMatch : IDecorationMatch
    {
        public Vector2Int SpawnPosition { get; set; }
        public GameObject ObjectToSpawn { get; set; }
        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }

}
