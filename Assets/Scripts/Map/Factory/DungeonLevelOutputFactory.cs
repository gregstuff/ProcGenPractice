
using System.Collections.Generic;
using System;

public static class DungeonLevelOutputFactory 
{

    private static Dictionary<DungeonLevelOutput, Func<DungeonOutputConfigSO, IDungeonOutput>> outputToInst =
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
        if (!outputToInst.TryGetValue(output, out var factory))
        {
            throw new Exception($"No dungeon generator found for {output}");
        }
        return factory(config);
    }


}
