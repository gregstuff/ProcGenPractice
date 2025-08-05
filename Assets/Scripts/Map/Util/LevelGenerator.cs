using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [SerializeField] private DungeonLevelGenerator selectedCalc;
    [SerializeField] private DungeonLevelOutput selectedOutput;
    [SerializeField] private GeneratedLevelLayoutSO generatedLevelLayout;
    [SerializeField] private DungeonOutputConfigSO outputConfig;

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        var dungeonGenerator = DungeonLevelGeneratorFactory.GetDungeonGenerator(selectedCalc);
        var output = DungeonLevelOutputFactory.GetDungeonOutput(selectedOutput, outputConfig);
        var level = dungeonGenerator.GenerateDungeonLevel(generatedLevelLayout);
        output.OutputMap(level);
    }

}
