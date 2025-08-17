using UnityEditor;
using UnityEngine;
using DungeonGeneration.Map.Enum;

public class TilePaletteUI
{
    private static readonly GUIStyle PALETTE_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5), padding = new RectOffset(10, 10, 10, 10) };
    private const float PALETTE_WIDTH = 150f;
    private const float PALETTE_ITEM_HEIGHT = 25f;
    private const float EXTRA_BOTTOM_SPACE = 10f;

    public static void Construct(TilePaletteUIModel tilePalette, Rect offsetRect)
    {
        // Add extra height to the Rect to ensure no clipping
        Rect adjustedRect = new Rect(offsetRect.x, offsetRect.y, offsetRect.width, offsetRect.height + EXTRA_BOTTOM_SPACE);

        GUI.BeginGroup(adjustedRect, PALETTE_STYLE);
        GUILayout.BeginVertical();
        GUILayout.Label("Tile Palette", EditorStyles.boldLabel);
        var tileTypes = System.Enum.GetValues(typeof(TileType));

        foreach (TileType tileType in tileTypes)
        {
            GUILayout.BeginHorizontal();
            Color? color = tilePalette.GetColorForTileType(tileType);
            GUI.backgroundColor = color ?? Color.white;
            GUILayout.Box("", GUILayout.Width(PALETTE_ITEM_HEIGHT), GUILayout.Height(PALETTE_ITEM_HEIGHT));
            GUI.backgroundColor = Color.white;
            GUILayout.Label($"{tileType.ToString()} : {tilePalette.GetKeyCodeForTileType(tileType)}");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUI.EndGroup();
    }

}