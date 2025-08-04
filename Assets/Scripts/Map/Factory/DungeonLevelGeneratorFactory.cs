
using System;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonLevelGeneratorFactory 
{

    private static Dictionary<DungeonLevelGeneratorVersion, Lazy<IDungeonLevelGenerator>> genVersionToInst = new Dictionary<DungeonLevelGeneratorVersion, Lazy<IDungeonLevelGenerator>>()
    { {DungeonLevelGeneratorVersion.DungeonRoomGenV1, new Lazy<IDungeonLevelGenerator>(() => new DungeonRoomGenV1())  } };


    public static IDungeonLevelGenerator GetDungeonGenerator(DungeonLevelGeneratorVersion version)
    {
        if (!genVersionToInst.TryGetValue(version, out var dungeonGenerator))
        {
            throw new Exception($"No dungeon generator found for {version}");
        }
        return dungeonGenerator.Value;
    }

}
