using UnityEditor;
using UnityEngine;
using System.Linq;

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
            select new DecorationRule()
            {
                Name = model.Name,
                MatchingPattern = model.MatchingPattern,
                SpawnCell = model.SpawnCell,
                PostSpawnBlockedCells = model.PostSpawnBlockedCells,
                Prefab = model.Prefab,
                SpawnScale = model.SpawnScale,
                SpawnRotation = model.SpawnRotation,
                SpawnPositionOffset = model.SpawnPositionOffset,
                MaxApplications = model.MaxApplications,
            };

        SOInstance.DecorationRules = decorationModels.ToArray();

        AssetDatabase.CreateAsset(SOInstance, path);
        AssetDatabase.SaveAssets();
    }

}