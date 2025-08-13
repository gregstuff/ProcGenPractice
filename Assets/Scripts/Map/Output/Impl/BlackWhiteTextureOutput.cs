using DungeonGeneration.Map.Model;
using DungeonGeneration.Map.Model.Rooms;
using DungeonGeneration.Map.Output.SO;
using UnityEngine;

namespace DungeonGeneration.Map.Output.Impl
{

    public class BlackWhiteTextureOutput : IDungeonOutput
    {

        private Renderer _renderer;

        public BlackWhiteTextureOutput(DungeonOutputConfigSO config)
        {
            _renderer = config.LevelLayoutDisplay;
        }

        public void OutputMap(RoomLevel level)
        {
            DrawLayout(level);
        }

        public void OutputMap(ILevel level)
        {
            throw new System.NotImplementedException();
        }

        void DrawLayout(RoomLevel level) 
        {
            var blockedGrid = level.GetBlockedMap();
            var layoutTexture = (Texture2D)_renderer.sharedMaterial.mainTexture;
            layoutTexture.Reinitialize(level.Width, level.Height);
            _renderer.transform.localScale = new Vector3(level.Width, level.Height, 1);
            layoutTexture.FillWithColor(Color.black);

            for (int y = 0; y < level.Height; ++y)
            {
                for (int x = 0; x < level.Height; ++x)
                {
                    layoutTexture.SetPixel(x, y, blockedGrid[y, x] ? Color.black : Color.white);
                }
            }

            layoutTexture.SaveAsset();
        }

    }
}