using UnityEditor;
using UnityEngine;
using System.Linq;

public class DecorationRulesetSO : ScriptableObject
{
    public DecorationRule[] DecorationRules;
    public TileMatchingRuleSetSO TileMatchingRules;

    public static void Construct(
        string path,
        DecorationRuleUIModel[] decorationUIModels,
        TileMatchingRuleSetSO TileMatchingRules)
    {
        var SOInstance = CreateInstance<DecorationRulesetSO>();

        var decorationModels =
            from model in decorationUIModels
            select new DecorationRule(model);
        
        SOInstance.DecorationRules = decorationModels.ToArray();
        SOInstance.TileMatchingRules = TileMatchingRules;

        AssetDatabase.CreateAsset(SOInstance, path);
        AssetDatabase.SaveAssets();
    }

}