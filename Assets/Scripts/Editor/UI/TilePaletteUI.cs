using ProcGenSys.Editor.Model;
using UnityEditor;
using UnityEngine;

namespace ProcGenSys.Editor.UI
{
    public class TilePaletteUI
    {
        private static readonly GUIStyle PALETTE_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5), padding = new RectOffset(10, 10, 10, 10) };
        private const float PALETTE_ITEM_HEIGHT = 25f;
        private const float EXTRA_BOTTOM_SPACE = 10f;
        private const float PALETTE_MIN_WIDTH = 250f;   // was 150

        public static void Construct(TilePaletteUIModel tilePalette, Rect offsetRect)
        {
            float width = Mathf.Max(offsetRect.width, PALETTE_MIN_WIDTH);
            Rect adjustedRect = new Rect(offsetRect.x, offsetRect.y, width, offsetRect.height + EXTRA_BOTTOM_SPACE);

            GUI.BeginGroup(adjustedRect, PALETTE_STYLE);
            GUILayout.BeginVertical();
            GUILayout.Label("Tile Palette", EditorStyles.boldLabel);

            foreach (var tileMatchingRule in tilePalette.TileMatchingRuleSet.TileMatchingRules)
            {
                GUILayout.BeginHorizontal();
                var color = tilePalette.GetColorForTileType(tileMatchingRule);
                GUI.backgroundColor = color ?? Color.white;
                GUILayout.Box("", GUILayout.Width(PALETTE_ITEM_HEIGHT), GUILayout.Height(PALETTE_ITEM_HEIGHT));
                GUI.backgroundColor = Color.white;

                // ensure the label gets room
                GUILayout.Label($"{tileMatchingRule.name} : {tilePalette.GetKeyCodeForTileType(tileMatchingRule)}",
                                GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUI.EndGroup();
        }


    }
}