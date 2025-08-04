using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{

    [SerializeField] private DungeonLevelGenerator selectedCalc;
    [SerializeField] private DungeonLevelOutput selectedOutput;
    [SerializeField] private GeneratedLevelLayoutSO generatedLevelLayout;

    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        var dungeonGenerator = DungeonLevelGeneratorFactory.GetDungeonGenerator(selectedCalc);
        var level = dungeonGenerator.GenerateDungeonLevel(generatedLevelLayout);
        //display the level with selected output
    }

}
