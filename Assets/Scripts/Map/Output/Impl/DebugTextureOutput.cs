using DungeonGeneration.Map.Enum;
using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.Output.SO;
using UnityEngine;

namespace DungeonGeneration.Map.Output.Impl
{
    public class DebugTextureOutput : IDungeonOutput
    {

        private Renderer _renderer;

        public DebugTextureOutput(DungeonOutputConfigSO config)
        {
            _renderer = config.LevelLayoutDisplay;
        }

        public void OutputMap(ILevel level)
        {
            var layoutTexture = (Texture2D)_renderer.sharedMaterial.mainTexture;
            layoutTexture.Reinitialize(level.Width, level.Height);
            _renderer.transform.localScale = new Vector3(level.Width, level.Height, 1);
            layoutTexture.FillWithColor(Color.black);

            for (int y = 0; y < level.Height; ++y)
            {
                for (int x = 0; x < level.Width; ++x)
                {
                    TileType tile = level.GetTileTypeAt(x, y);
                    Color relevantColor = GetColorForTileType(tile);
                    layoutTexture.DrawPixel(new Vector2Int { x = x, y = y }, relevantColor);
                }
            }

            layoutTexture.SaveAsset();
        }

        private Color GetColorForTileType(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.None:
                    return Color.black;
                case TileType.Room:
                    return Color.white;
                case TileType.Hallway:
                    return Color.grey;
                case TileType.Door:
                    return Color.red;
                default:
                    return Color.black;
            }
        }

    }

}