using UnityEditor;
using UnityEngine;

public class RuleCardUI
{

    private static GUIStyle CARD_STYLE 
        = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5) };

    public static void Construct(
        GridRule gridRule,
        TilePalette tilePalette,
        System.Action onDeleteClicked,
        System.Action<Vector2> onResizeStarted)
    {
        var ( gridWidth, gridHeight, gridPattern, gridID ) = gridRule;
        
        GUILayout.BeginVertical(CARD_STYLE,
            GUILayout.Width(gridWidth * ProcGenRulesWindowConstants.CELL_LENGTH
            + ProcGenRulesWindowConstants.CELL_PADDING));

        EditorGUILayout.TextField("ID", gridID);

        HandleGrid(gridRule, tilePalette);

        HandleResizing(onResizeStarted);

        // Duplicate and Delete buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete"))
        {
            onDeleteClicked?.Invoke();
        }
        GUILayout.EndHorizontal();
        

        GUILayout.EndVertical();
    }

    private static void HandleResizing(System.Action<Vector2> onResizeStarted)
    {
        Rect handleRect = GUILayoutUtility.GetLastRect();

        // Hover and drag for resize handle
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

    private static void HandleGrid(GridRule gridRule, TilePalette tilePalette)
    {
        var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;

        for (int y = 0; y < gridHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                Color color = tilePalette.GetColorForTile(gridPattern[y, x]);
                GUI.backgroundColor = color;
                GUILayout.Box("", GUILayout.Width(ProcGenRulesWindowConstants.CELL_LENGTH),
                    GUILayout.Height(ProcGenRulesWindowConstants.CELL_LENGTH));
                GUI.backgroundColor = Color.white;

                // Hover detection for grid cells
                Rect cellRect = GUILayoutUtility.GetLastRect();
                if (cellRect.Contains(Event.current.mousePosition))
                {
                    EditorGUIUtility.AddCursorRect(cellRect, MouseCursor.Link);
                }
            }
            GUILayout.EndHorizontal();
        }

        // Black resize handle, left-aligned
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.black;
        GUILayout.Box("", GUILayout.Width(ProcGenRulesWindowConstants.CELL_LENGTH),
            GUILayout.Height(ProcGenRulesWindowConstants.CELL_LENGTH));
        GUI.backgroundColor = Color.white;
        GUILayout.FlexibleSpace(); // Push remaining space to right
        GUILayout.EndHorizontal();
    }


}
