using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProcGenSys.Pipeline.LevelDataGeneration.Model
{
    public enum WallSide
    {
        UNDEFINED,
        FRONT,
        LEFT,
        RIGHT,
        BACK
    }

    public static class WallSideExtension
    {
        private static Color yellow = new Color(1, 1, 0, 1);
        private static readonly Dictionary<WallSide, Color> DirectionToColorMap = new Dictionary<WallSide, Color>()
    {
        { WallSide.FRONT, Color.cyan },
        { WallSide.LEFT, yellow },
        { WallSide.RIGHT, Color.magenta },
        { WallSide.BACK, Color.green },
    };

        public static WallSide GetDirectionForColor(Color candidate)
        {
            var invertedDict = DirectionToColorMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            return invertedDict.TryGetValue(candidate, out var direction) ? direction : WallSide.UNDEFINED;
        }

        public static Color GetColor(this WallSide direction)
        {
            return DirectionToColorMap.TryGetValue(direction, out Color color) ? color : Color.white;
        }

        public static WallSide GetOpposite(this WallSide direction)
        {
            switch (direction)
            {
                case WallSide.LEFT: return WallSide.RIGHT;
                case WallSide.RIGHT: return WallSide.LEFT;
                case WallSide.FRONT: return WallSide.BACK;
                case WallSide.BACK: return WallSide.FRONT;
                default: return WallSide.UNDEFINED;
            }
        }


    }

}