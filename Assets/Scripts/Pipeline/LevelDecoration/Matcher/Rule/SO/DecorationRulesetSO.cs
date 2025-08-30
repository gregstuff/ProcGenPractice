using ProcGenSys.Editor.Model;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDecoration.Matcher.Rule
{
    public class DecorationRulesetSO : ScriptableObject
    {
        public DecorationRule[] DecorationRules;
        public TileMatchingRuleSetSO TileMatchingRuleSet;

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
            SOInstance.TileMatchingRuleSet = TileMatchingRules;

            AssetDatabase.CreateAsset(SOInstance, path);
            AssetDatabase.SaveAssets();
        }

    }
}