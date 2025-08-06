using UnityEngine;

public class DebugTextureOutput : IDungeonOutput
{

    private Renderer _renderer;

    public DebugTextureOutput(DungeonOutputConfigSO config)
    {
        _renderer = config.LevelLayoutDisplay;
    }

    public void OutputMap(Level level)
    {
        DrawLayout(level);
    }


    void DrawLayout(Level level)
    {
        var layoutTexture = (Texture2D)_renderer.sharedMaterial.mainTexture;
        layoutTexture.Reinitialize(level.Width, level.Height);
        _renderer.transform.localScale = new Vector3(level.Width, level.Height, 1);
        layoutTexture.FillWithColor(Color.black);

        level.Hallways.ForEach(hallway => layoutTexture.DrawLine(hallway.PointOne, hallway.PointTwo, Color.white));

        level.Rooms.ForEach(room =>
        {
            if (room.LayoutTexture == null)
            {
                layoutTexture.DrawRectangle(room.Area, Color.white);
            }
            else
            {
                layoutTexture.DrawTexture(room.LayoutTexture, room.Area);
            }

            room.Doors.ForEach(existingDoor => layoutTexture.DrawPixel(room.GetAbsolutePositionForDoor(existingDoor), Color.green));
            room.PossibleDoors.ForEach(potentialDoor => layoutTexture.DrawPixel(room.GetAbsolutePositionForDoor(potentialDoor), Color.red));
        });

        layoutTexture.SaveAsset();
    }

}
