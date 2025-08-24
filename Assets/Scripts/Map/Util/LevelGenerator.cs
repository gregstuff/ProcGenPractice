using DungeonGeneration.Map.Output;
using UnityEngine;

namespace DungeonGeneration.Map.Util
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private LevelGeneratorSO levelGenerator;
        [SerializeField] private OutputGeneratorSO outputGenerator;
        [SerializeField] private DecoratorSO decorator;

        [ContextMenu("Generate Level Layout")]
        public void GenerateLevel()
        {
            var level = levelGenerator.GenerateLevel();
            outputGenerator.OutputMap(level);
            decorator.ApplyDecorations(level);
        }
    }
}