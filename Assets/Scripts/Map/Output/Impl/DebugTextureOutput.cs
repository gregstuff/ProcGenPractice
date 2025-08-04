
public class DebugTextureOutput : IMapGenOutput
{
    public void OutputMap()
    {



    }

    /*
    void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidate = new RectInt(), bool isDebug = false)
    {
        var renderer = levelLayoutDisplay.GetComponent<Renderer>();

        var layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
        layoutTexture.Reinitialize(levelConfig.Width, levelConfig.Height);
        levelLayoutDisplay.transform.localScale = new Vector3(levelConfig.Width, levelConfig.Height, 1);
        layoutTexture.FillWithColor(Color.black);

        Array.ForEach(_level.Rooms, (room) =>
        {
            if (room.LayoutTexture == null)
            {
                layoutTexture.DrawRectangle(room.Area, Color.white);
            }
            else
            {
                layoutTexture.DrawTexture(room.LayoutTexture, room.Area);
            }

        });
        Array.ForEach(_level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));

        layoutTexture.ConvertToBlackAndWhite();

        if (isDebug)
        {
            openDoorways.ForEach(h => layoutTexture.SetPixel(h.StartPositionAbsolute.x, h.StartPositionAbsolute.y, h.StartDirection.GetColor()));

            if (selectedEntryway != null)
                layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x,
                    selectedEntryway.StartPositionAbsolute.y,
                    Color.white);
        }


        layoutTexture.SaveAsset();
    }
    */
}
