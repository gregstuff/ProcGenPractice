using ProcGenSys.Pipeline.LevelDataGeneration.Generators;
using ProcGenSys.Pipeline.LevelDecoration;
using ProcGenSys.Pipeline.LevelGeometryGeneration;
using UnityEngine;

namespace ProcGenSys.Common.Util
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