using DungeonGeneration.Map.Output;
using UnityEngine;

namespace DungeonGeneration.Map.Util
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] private ILevelGenerator levelGenerator;
        [SerializeField] private IOutputGenerator outputGenerator;
        [SerializeField] private IDecorator decorator;

        [ContextMenu("Generate Level Layout")]
        public void GenerateLevel()
        {
            var level = levelGenerator.GenerateLevel();
            outputGenerator.OutputMap(level);
            decorator.ApplyDecorations(level);
        }
    }
}