using DungeonGeneration.Map.Factory.Enum;
using DungeonGeneration.Map.Gen;
using DungeonGeneration.Map.Gen.Impl;
using System;
using System.Collections.Generic;

namespace DungeonGeneration.Map.Factory
{
    public static class DungeonLevelGeneratorFactory
    {

        private static Dictionary<DungeonLevelGenerator, Lazy<IDungeonLevelGenerator>> generatorMap =
            new Dictionary<DungeonLevelGenerator, Lazy<IDungeonLevelGenerator>>()
            {
            {
                DungeonLevelGenerator.DungeonRoomGen, new Lazy<IDungeonLevelGenerator>(() => new DungeonRoomGen())
            }
            };

        public static IDungeonLevelGenerator GetDungeonGenerator(DungeonLevelGenerator version)
        {
            if (!generatorMap.TryGetValue(version, out var dungeonGenerator))
            {
                throw new KeyNotFoundException($"No dungeon generator found for {version}");
            }
            return dungeonGenerator.Value;
        }

    }
}