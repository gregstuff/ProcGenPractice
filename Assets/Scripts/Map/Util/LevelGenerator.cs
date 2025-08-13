using DungeonGeneration.Map.Factory;
using DungeonGeneration.Map.Factory.Enum;
using DungeonGeneration.Map.Output.SO;
using DungeonGeneration.Map.SO;
using UnityEngine;
using Zenject;

namespace DungeonGeneration.Map.Util
{

    public class LevelGenerator : MonoBehaviour
    {

        [SerializeField] private DungeonLevelGenerator selectedCalc;
        [SerializeField] private DungeonLevelOutput selectedOutput;
        [SerializeField] private DecoratorType selectedDecorator;
        [SerializeField] private GeneratedLevelLayoutSO generatedLevelLayout;
        [SerializeField] private DungeonOutputConfigSO outputConfig;

        [ContextMenu("Generate Level Layout")]
        public void GenerateLevel()
        {
            var dungeonGenerator = DungeonLevelGeneratorFactory.GetDungeonGenerator(selectedCalc);
            var level = dungeonGenerator.GenerateDungeonLevel(generatedLevelLayout);
            DungeonLevelOutputFactory.GetDungeonOutput(selectedOutput, outputConfig).OutputMap(level);
            DecoratorFactory.GetDecorator(selectedDecorator).ApplyDecorations(level, outputConfig);
        }
    }
}