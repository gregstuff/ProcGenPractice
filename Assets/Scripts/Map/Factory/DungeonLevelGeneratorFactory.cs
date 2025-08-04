
using System;
using System.Collections.Generic;
using UnityEngine;

public static class DungeonLevelGeneratorFactory 
{

    private static Dictionary<DungeonLevelGenerator, Lazy<IDungeonLevelGenerator>> genVersionToInst = new Dictionary<DungeonLevelGenerator, Lazy<IDungeonLevelGenerator>>()
    { {DungeonLevelGenerator.DungeonRoomGenV1, new Lazy<IDungeonLevelGenerator>(() => new DungeonRoomGen())  } };


    public static IDungeonLevelGenerator GetDungeonGenerator(DungeonLevelGenerator version)
    {
        if (!genVersionToInst.TryGetValue(version, out var dungeonGenerator))
        {
            throw new Exception($"No dungeon generator found for {version}");
        }
        return dungeonGenerator.Value;
    }

}
