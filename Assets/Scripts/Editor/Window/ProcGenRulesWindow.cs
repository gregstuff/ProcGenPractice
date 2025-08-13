using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DungeonGeneration.Map.Enum;

public class ProcGenRulesWindow : EditorWindow
{
    private List<Rule> rules = new List<Rule>();
    private Vector2 scrollPosition;
    private static readonly int CELL_LENGTH = 20;
    private static readonly int PADDING = 10;
    private Vector2 dragStartPos;
    private int startWidth, startHeight;
    private int clickedRuleIndex = -1;
    private bool isDragging = false;

    [MenuItem("Window/Proc Gen Rules")]
    public static void ShowWindow()
    {
        GetWindow<ProcGenRulesWindow>("Proc Gen Rules Editor");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Rule"))
        {
            var newRule = new Rule();
            rules.Add(newRule);
        }
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];
            // Set rule card width to grid width + padding
            GUIStyle cardStyle = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5) };
            GUILayout.BeginVertical(cardStyle, GUILayout.Width(rule.width * CELL_LENGTH + PADDING));
            rule.foldout = EditorGUILayout.Foldout(rule.foldout, rule.id, true);
            if (rule.foldout)
            {
                rule.id = EditorGUILayout.TextField("ID", rule.id);

                GUILayout.Label("Grid Pattern Preview:");
                for (int y = 0; y < rule.height; y++)
                {
                    GUILayout.BeginHorizontal();
                    for (int x = 0; x < rule.width; x++)
                    {
                        Color color = GetColorForTile(rule.pattern[y, x]);
                        GUI.backgroundColor = color;
                        GUILayout.Box("", GUILayout.Width(CELL_LENGTH), GUILayout.Height(CELL_LENGTH));
                        GUI.backgroundColor = Color.white;

                        // Hover detection
                        Rect cellRect = GUILayoutUtility.GetLastRect();
                        if (cellRect.Contains(Event.current.mousePosition))
                        {
                            EditorGUIUtility.AddCursorRect(cellRect, MouseCursor.Link);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                // Black resize handle on next line, left-aligned
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.black;
                GUILayout.Box("", GUILayout.Width(CELL_LENGTH), GUILayout.Height(CELL_LENGTH));
                GUI.backgroundColor = Color.white;
                Rect handleRect = GUILayoutUtility.GetLastRect();
                GUILayout.FlexibleSpace(); // Push remaining space to right
                GUILayout.EndHorizontal();

                // Hover and drag for resize handle
                if (handleRect.Contains(Event.current.mousePosition))
                {
                    EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.ResizeUpLeft);
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        clickedRuleIndex = i;
                        dragStartPos = Event.current.mousePosition;
                        startWidth = rule.width;
                        startHeight = rule.height;
                        isDragging = false;
                        GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                        Event.current.Use();
                    }
                }

                GUILayout.Label("Metadata:");
                rule.maxApplications = EditorGUILayout.IntField("Max Applications (-1 infinite)", rule.maxApplications);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Duplicate")) { /* Implement duplicate */ }
                if (GUILayout.Button("Delete")) { rules.RemoveAt(i); }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        // Drag handling for resize handle
        if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl != 0 && clickedRuleIndex != -1)
        {
            var rule = rules[clickedRuleIndex];
            Vector2 delta = Event.current.mousePosition - dragStartPos;
            int addCols = (int)(delta.x / CELL_LENGTH);
            int addRows = (int)(delta.y / CELL_LENGTH);
            int newWidth = Mathf.Max(3, startWidth + addCols);
            int newHeight = Mathf.Max(3, startHeight + addRows);
            if (newWidth != rule.width || newHeight != rule.height)
            {
                rule.ResizeGrid(newHeight, newWidth);
                isDragging = true;
                Repaint();
            }
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl != 0 && clickedRuleIndex != -1)
        {
            GUIUtility.hotControl = 0;
            clickedRuleIndex = -1;
            isDragging = false;
            Event.current.Use();
        }

        GUILayout.EndScrollView();
    }

    private Color GetColorForTile(TileType type)
    {
        switch (type)
        {
            case TileType.None: return Color.green;
            case TileType.Hallway: return Color.blue;
            case TileType.Room: return Color.gray;
            default: return Color.white;
        }
    }
}