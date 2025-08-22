using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        foreach (TileMatchingRuleSO tileMatchingRule in tilePalette.TileMatchingRuleSet.TileMatchingRules)
        {
            GUILayout.BeginHorizontal();
            Color? color = tilePalette.GetColorForTileType(tileMatchingRule);
            GUI.backgroundColor = color ?? Color.white;
            GUILayout.Box("", GUILayout.Width(PALETTE_ITEM_HEIGHT), GUILayout.Height(PALETTE_ITEM_HEIGHT));
            GUI.backgroundColor = Color.white;
            GUILayout.Label($"{tileMatchingRule.Name} : {tilePalette.GetKeyCodeForTileType(tileMatchingRule)}");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUI.EndGroup();
    }

}