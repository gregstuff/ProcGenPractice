
using System;
using UnityEngine;

public class DebugTextureOutput : IDungeonOutput
{

    private Renderer _renderer;

    public DebugTextureOutput(Renderer renderer)
    {
        _renderer = renderer;
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

            room.Doors.ForEach(door => layoutTexture.DrawPixel(door.AbsolutePosition, Color.green));
            room.PossibleDoors.ForEach(door => layoutTexture.DrawPixel(door.AbsolutePosition, Color.red));

        });

        //Array.ForEach(_level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        //layoutTexture.ConvertToBlackAndWhite();

        /*
        if (isDebug)
        {
            openDoorways.ForEach(h => layoutTexture.SetPixel(h.StartPositionAbsolute.x, h.StartPositionAbsolute.y, h.StartDirection.GetColor()));

            if (selectedEntryway != null)
                layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x,
                    selectedEntryway.StartPositionAbsolute.y,
                    Color.white);
        }
        */

        layoutTexture.SaveAsset();
    }

}
