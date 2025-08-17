using UnityEditor;
using UnityEngine;
using System.Linq;
using DungeonGeneration.Map.Enum;

public class DecorationRulesetSO : ScriptableObject
{
    public DecorationRule[] DecorationRules;

    public static void Construct(
        string path,
        DecorationRuleUIModel[] decorationUIModels)
    {
        var SOInstance = CreateInstance<DecorationRulesetSO>();
        var decorationModels =
            from model in decorationUIModels
            select new DecorationRule(model);

        SOInstance.DecorationRules = decorationModels.ToArray();
        AssetDatabase.CreateAsset(SOInstance, path);
        AssetDatabase.SaveAssets();
    }

}