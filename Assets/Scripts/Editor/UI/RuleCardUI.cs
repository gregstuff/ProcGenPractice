using UnityEditor;
using UnityEngine;
using DungeonGeneration.Map.Enum;

public class RuleCardUI
{
    private static GUIStyle CARD_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5) };
    private static GUIStyle RESIZE_HANDLE_STYLE = new GUIStyle("box")
    {
        fontSize = 14,
        alignment = TextAnchor.MiddleCenter,
        normal = { textColor = Color.white, background = Texture2D.grayTexture },
        hover = { background = Texture2D.whiteTexture },
        margin = new RectOffset(0, 0, 0, 0),
        padding = new RectOffset(0, 0, 0, 0),
        fixedWidth = ProcGenRulesWindowConstants.CELL_LENGTH,
        fixedHeight = ProcGenRulesWindowConstants.CELL_LENGTH
    };
    private static GUIStyle CURSOR_STYLE = new GUIStyle("box")
    {
        fixedWidth = ProcGenRulesWindowConstants.CELL_LENGTH,
        fixedHeight = ProcGenRulesWindowConstants.CELL_LENGTH
    };
    private static TileType? selectedTileType = null;

    public static void Construct(
        DecorationRuleUIModel gridRule,
        TilePaletteUIModel tilePalette,
        System.Action onDeleteClicked,
        System.Action<Vector2> onResizeStarted,
        bool isCollapsed,
        System.Action<bool> onToggleCollapse)
    {
        var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;
        GUILayout.BeginVertical(CARD_STYLE,
            GUILayout.Width(gridWidth * ProcGenRulesWindowConstants.CELL_LENGTH
            + ProcGenRulesWindowConstants.CELL_PADDING));

        GUILayout.BeginHorizontal(GUILayout.Width(gridWidth * ProcGenRulesWindowConstants.CELL_LENGTH
            + ProcGenRulesWindowConstants.CELL_PADDING));
        bool newCollapsed = !EditorGUILayout.Foldout(!isCollapsed, $"Rule ID: {gridID}", true);
        GUILayout.EndHorizontal();

        if (newCollapsed != isCollapsed)
        {
            onToggleCollapse?.Invoke(newCollapsed);
        }

        if (!newCollapsed)
        {
            EditorGUILayout.TextField("ID", gridID);
            HandleGrid(gridRule, tilePalette);
            HandleResizing(onResizeStarted);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete"))
            {
                onDeleteClicked?.Invoke();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }

    private static void HandleResizing(System.Action<Vector2> onResizeStarted)
    {
        Rect handleRect = GUILayoutUtility.GetLastRect();
        if (handleRect.Contains(Event.current.mousePosition))
        {
            EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.ResizeUpLeft);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                onResizeStarted?.Invoke(Event.current.mousePosition);
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Event.current.Use();
            }
        }
    }

    private static void HandleGrid(
        DecorationRuleUIModel gridRule,
        TilePaletteUIModel tilePalette)
    {
        var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;

        // Handle key presses for tile selection
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                selectedTileType = null;
                Event.current.Use();
            }
            else
            {
                TileType? tileType = tilePalette.GetTileTypeForKeyCode(Event.current.keyCode);
                if (tileType.HasValue)
                {
                    selectedTileType = tileType.Value;
                    Event.current.Use();
                }
            }
        }

        for (int y = 0; y < gridHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                Color color = tilePalette.GetColorForTileType(gridPattern[y, x]) ?? Color.white;
                GUI.backgroundColor = color;
                GUILayout.Box("", GUILayout.Width(ProcGenRulesWindowConstants.CELL_LENGTH),
                    GUILayout.Height(ProcGenRulesWindowConstants.CELL_LENGTH));
                GUI.backgroundColor = Color.white;

                Rect cellRect = GUILayoutUtility.GetLastRect();
                if (cellRect.Contains(Event.current.mousePosition))
                {
                    // Set cursor if a tile type is selected
                    if (selectedTileType.HasValue)
                    {
                        CURSOR_STYLE.normal.background = Texture2D.whiteTexture;
                        GUI.backgroundColor = tilePalette.GetColorForTileType(selectedTileType.Value) ?? Color.white;
                        GUI.Box(cellRect, "", CURSOR_STYLE);
                        GUI.backgroundColor = Color.white;
                    }

                    // Paint on click or hold
                    if (selectedTileType.HasValue && Event.current.isMouse &&
                        (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&
                        Event.current.button == 0)
                    {
                        gridPattern[y, x] = selectedTileType.Value;
                        Event.current.Use();
                        EditorWindow.GetWindow<ProcGenRulesWindowUI>().Repaint();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Box("↘", RESIZE_HANDLE_STYLE);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}