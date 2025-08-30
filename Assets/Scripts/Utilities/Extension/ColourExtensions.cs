
using UnityEngine;

namespace ProcGenSys.Utilities.Extension
{
    public static class ColourExtensions
    {
        private static readonly Color[] UnityDefaults = new Color[]
        {
        Color.black,
        Color.blue,
        Color.clear,
        Color.cyan,
        Color.gray,
        Color.green,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow
        };

        public static Color SnapToNearest(this Color input)
        {
            Color closest = UnityDefaults[0];
            float minDistSqr = float.MaxValue;

            foreach (var color in UnityDefaults)
            {
                // Use Vector4 for RGBA distance (or Vector3 for RGB-only: new Vector3(input.r, input.g, input.b))
                Vector4 diff = new Vector4(input.r - color.r, input.g - color.g, input.b - color.b, input.a - color.a);
                float distSqr = diff.sqrMagnitude;

                if (distSqr < minDistSqr)
                {
                    minDistSqr = distSqr;
                    closest = color;
                }
            }

            return closest;
        }


    }
}