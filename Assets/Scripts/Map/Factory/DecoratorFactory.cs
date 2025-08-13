using System.Collections.Generic;
using System;
using DungeonGeneration.Map.Factory.Enum;

public static class DecoratorFactory
{
    private static Dictionary<DecoratorType, Lazy<IDecorator>> decoratorMap =
        new Dictionary<DecoratorType, Lazy<IDecorator>>()
        {
        {
            DecoratorType.Base, new Lazy<IDecorator>(() => new Decorator())
        }
        };

    public static IDecorator GetDecorator(DecoratorType version)
    {
        if (!decoratorMap.TryGetValue(version, out var decorator))
        {
            throw new KeyNotFoundException($"No dungeon generator found for {version}");
        }
        return decorator.Value;
    }
}
