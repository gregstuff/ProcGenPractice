using UnityEngine;

[CreateAssetMenu(menuName = "ProcGen/Decorators/Matchers/Grid Decoration Matcher")]
public class GridDecorationMatcherSO : DecorationMatcherSO
{
    [SerializeField] private DecorationRulesetSO _decorationRuleset;

    public override IDecorationMatch[] GetDecorationMatchers(ICapabilityProvider level)
    {
        return null;
    }
}
