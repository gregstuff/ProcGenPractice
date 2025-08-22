using UnityEngine;
using System;
using DungeonGeneration.Service.Util;

namespace DungeonGeneration.Map.Output.Impl
{

    public class BlackWhiteTextureOutput : ScriptableObject, IOutputGenerator
    {
        [SerializeField] private Renderer _levelLayoutRenderer;

        public void OutputMap(ICapabilityProvider level)
        {
            if(!level.TryGet<IBlockMask>(out var blockedMap)
                ||!level.TryGet<IDimensions>(out var dimensions))
            {
                throw new Exception($"Selected level generation missing required inputs for ${typeof(BlackWhiteTextureOutput)}");
            }

            DrawLayout(blockedMap.Mask, dimensions.MapDimensions);
        }

        void DrawLayout(bool[,] blockedMap, Vector2Int mapDimensions) 
        {

            var height = mapDimensions.y;
            var width = mapDimensions.x;
            var renderer = ObjectSpawnerSingleton.Instance.Spawn(_levelLayoutRenderer);

            var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
            layoutTexture.Reinitialize(width, height);
            renderer.transform.localScale = new Vector3(width, height, 1);
            layoutTexture.FillWithColor(Color.black);

            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < height; ++x)
                {
                    layoutTexture.SetPixel(x, y, blockedMap[y, x] ? Color.black : Color.white);
                }
            }

            layoutTexture.SaveAsset();
        }

    }
}