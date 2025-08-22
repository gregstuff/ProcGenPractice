using DungeonGeneration.Map.Output.SO;
using System;
using UnityEngine;

namespace DungeonGeneration.Map.Output.Impl
{
    public class DebugTextureOutput : ScriptableObject, IOutputGenerator
    {
        [SerializeField] DungeonOutputConfigSO _config;

        public void OutputMap(ICapabilityProvider level)
        {

            if (!level.TryGet<IDimensions>(out var dimensions)
                ||!level.TryGet<ITileLayer>(out var tileLayer))
            {
                throw new ArgumentException($"Level generation is missing required data for ${typeof(DebugTextureOutput)}");
            }

            var width = dimensions.MapDimensions.x;
            var height = dimensions.MapDimensions.y;
            var renderer = _config.LevelLayoutDisplay;
            var tileTypes = tileLayer.Tiles;
            
            var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
            layoutTexture.Reinitialize(width, height);
            renderer.transform.localScale = new Vector3(width, height, 1);
            layoutTexture.FillWithColor(Color.black);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    TileTypeSO tileType = tileTypes[y,x];
                    Color relevantColor = GetColorForTileTypeTag(tileType);
                    layoutTexture.DrawPixel(new Vector2Int { x = x, y = y }, relevantColor);
                }
            }

            layoutTexture.SaveAsset();
        }

        private Color GetColorForTileTypeTag(TileTypeSO tileType)
        {
            if(tileType.HasTag(TileTag.None)) return Color.black;
            else if(tileType.HasTag(TileTag.Room)) return Color.white;
            else if(tileType.HasTag(TileTag.Hallway)) return Color.grey;
            else if(tileType.HasTag(TileTag.Door)) return Color.red;
            else return Color.black;
        }

    }

}