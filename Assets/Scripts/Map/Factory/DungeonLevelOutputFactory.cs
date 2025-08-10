using System.Collections.Generic;
using System;
using DungeonGeneration.Map.Enum;
using DungeonGeneration.Map.Output.SO;
using DungeonGeneration.Map.Output;
using DungeonGeneration.Map.Output.Impl;

namespace DungeonGeneration.Map.Factory
{
    public static class DungeonLevelOutputFactory
    {

        private static Dictionary<DungeonLevelOutput, Func<DungeonOutputConfigSO, IDungeonOutput>> outputMap =
        new Dictionary<DungeonLevelOutput, Func<DungeonOutputConfigSO, IDungeonOutput>>()
        {
        {
            DungeonLevelOutput.DebugConsoleOutput, config => new DebugConsoleOutput()
        },
        {
            DungeonLevelOutput.DebugTextureOutput, config => new DebugTextureOutput(config)
        },
        {
            DungeonLevelOutput.BlackWhiteTextureOutput, config => new BlackWhiteTextureOutput(config)
        },
        {
            DungeonLevelOutput.TileMapOutput3d, config => new TileMapOutput3d(config)
        },
        };

        public static IDungeonOutput GetDungeonOutput(DungeonLevelOutput output, DungeonOutputConfigSO config)
        {
            if (!outputMap.TryGetValue(output, out var factory))
            {
                throw new KeyNotFoundException($"No dungeon generator found for {output}");
            }
            return factory(config);
        }


    }
}
