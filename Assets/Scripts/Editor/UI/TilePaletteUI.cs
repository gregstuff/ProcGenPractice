using UnityEditor;
using UnityEngine;
using DungeonGeneration.Map.Enum;

public class TilePaletteUI
{
    private static readonly GUIStyle PALETTE_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5), padding = new RectOffset(10, 10, 10, 10) };
    private const float PALETTE_WIDTH = 150f;
    private const float PALETTE_ITEM_HEIGHT = 25f; // Increased from 20f to 25f
    private const float EXTRA_BOTTOM_SPACE = 10f; // Extra space to prevent clipping

    public static void Construct(TilePalette tilePalette, Rect offsetRect)
    {
        // Add extra height to the Rect to ensure no clipping
        Rect adjustedRect = new Rect(offsetRect.x, offsetRect.y, offsetRect.width, offsetRect.height + EXTRA_BOTTOM_SPACE);

        GUI.BeginGroup(adjustedRect, PALETTE_STYLE);
        GUILayout.BeginVertical();
        GUILayout.Label("Tile Palette", EditorStyles.boldLabel);
        foreach (TileType tileType in System.Enum.GetValues(typeof(TileType)))
        {
            GUILayout.BeginHorizontal();
            Color color = tilePalette.GetColorForTile(tileType);
            GUI.backgroundColor = color;
            GUILayout.Box("", GUILayout.Width(PALETTE_ITEM_HEIGHT), GUILayout.Height(PALETTE_ITEM_HEIGHT));
            GUI.backgroundColor = Color.white;
            GUILayout.Label(tileType.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUI.EndGroup();
    }
}