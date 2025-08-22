using UnityEditor;
using UnityEngine;
using DungeonGeneration.Map.Enum;
using System;

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
    private static GUIStyle boldStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };
    private static TileMatchingRuleSO? selectingMatchingTile = null;
    private enum GridTab { Matching, Spawning, Blocking }
    private static GridTab currentTab = GridTab.Matching;
    private static bool _isBlockPaintActive = false;
    private static bool _blockPaintTargetValue = false; // true = paint blocked, false = erase

    public static void Construct(
        DecorationRuleUIModel gridRule,
        TilePaletteUIModel tilePalette,
        Action onDeleteClicked,
        Action<Vector2, DecorationRuleUIModel> onResizeStarted,
        bool isCollapsed,
        Action<bool> onToggleCollapse)
    {
        var (_, gridWidth, gridHeight, gridPattern, spawnGrid, blockGrid) = gridRule;
        GUILayout.BeginVertical(CARD_STYLE,
            GUILayout.Width(gridWidth * ProcGenRulesWindowConstants.CELL_LENGTH
            + ProcGenRulesWindowConstants.CELL_PADDING));
        GUILayout.BeginHorizontal(GUILayout.Width(gridWidth * ProcGenRulesWindowConstants.CELL_LENGTH
            + ProcGenRulesWindowConstants.CELL_PADDING));
        bool newCollapsed = !EditorGUILayout.Foldout(!isCollapsed, $"Rule ID: {gridRule.Name}", true);
        GUILayout.EndHorizontal();
        if (newCollapsed != isCollapsed)
        {
            onToggleCollapse?.Invoke(newCollapsed);
        }
        if (!newCollapsed)
        {
            gridRule.Name = EditorGUILayout.TextField("ID", gridRule.Name);
            currentTab = (GridTab)GUILayout.Toolbar((int)currentTab, new[] { "Matching Grid", "Spawning Grid", "Blocking Grid" });
            if (currentTab == GridTab.Matching)
            {
                HandleMatchingGridLabel();
                HandleMatchingGrid(gridRule, tilePalette);
                HandleMatchingGridResizing(gridRule, onResizeStarted);
            }
            else if (currentTab == GridTab.Spawning)
            {
                HandleSpawningGridLabel();
                HandleSpawningGrid(gridRule);
            }
            else
            {
                HandleBlockingGridLabel();
                HandleBlockingGrid(gridRule);
            }
            HandleSpawnedPrefabField(gridRule);
            HandleSpawnScaleField(gridRule);
            HandleSpawnRotationField(gridRule);
            HandleSpawnOffsetField(gridRule);
            HandleDeleteButton(onDeleteClicked);
        }
        GUILayout.EndVertical();
    }

    private static void HandleMatchingGridLabel()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Matching Pattern", boldStyle);
        GUILayout.EndHorizontal();
    }

    private static void HandleSpawningGridLabel()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Spawning Grid", boldStyle);
        GUILayout.EndHorizontal();
    }

    private static void HandleBlockingGridLabel()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Blocking Grid", boldStyle);
        GUILayout.EndHorizontal();
    }

    private static void HandleMatchingGridResizing(
        DecorationRuleUIModel gridRule,
        System.Action<Vector2, DecorationRuleUIModel> onResizeStarted)
    {
        Rect handleRect = GUILayoutUtility.GetLastRect();
        if (handleRect.Contains(Event.current.mousePosition))
        {
            EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.ResizeUpLeft);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                onResizeStarted?.Invoke(Event.current.mousePosition, gridRule);
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Event.current.Use();
            }
        }
    }

    private static void HandleMatchingGrid(
        DecorationRuleUIModel gridRule,
        TilePaletteUIModel tilePalette)
    {
        var (_, gridWidth, gridHeight, gridPattern, _, _) = gridRule;
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                selectingMatchingTile = null;
                Event.current.Use();
            }
            else
            {
                TileMatchingRuleSO tileType = tilePalette.GetTileTypeForKeyCode(Event.current.keyCode);
                if (tileType != null)
                {
                    selectingMatchingTile = tileType;
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
                    if (selectingMatchingTile != null)
                    {
                        CURSOR_STYLE.normal.background = Texture2D.whiteTexture;
                        GUI.backgroundColor = tilePalette.GetColorForTileType(selectingMatchingTile) ?? Color.white;
                        GUI.Box(cellRect, "", CURSOR_STYLE);
                        GUI.backgroundColor = Color.white;
                    }
                    if (selectingMatchingTile != null && Event.current.isMouse &&
                        (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&
                        Event.current.button == 0)
                    {
                        gridPattern[y, x] = selectingMatchingTile;
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

    private static void HandleSpawningGrid(
        DecorationRuleUIModel gridRule)
    {
        var (_, gridWidth, gridHeight, _, spawnGrid, blockingGrid) = gridRule;
        for (int y = 0; y < gridHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                GUI.backgroundColor = spawnGrid[y, x] ? Color.red : Color.black;
                GUILayout.Box("", GUILayout.Width(ProcGenRulesWindowConstants.CELL_LENGTH),
                    GUILayout.Height(ProcGenRulesWindowConstants.CELL_LENGTH));
                GUI.backgroundColor = Color.white;
                Rect cellRect = GUILayoutUtility.GetLastRect();
                if (cellRect.Contains(Event.current.mousePosition) 
                    && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) 
                    && Event.current.button == 0)
                {
                    for (int i = 0; i < gridHeight; i++)
                        for (int j = 0; j < gridWidth; j++)
                            spawnGrid[i, j] = false;
                    spawnGrid[y, x] = true;
                    blockingGrid[y, x] = true; //if we're spawning here then it's blocked here
                    gridRule.SpawnCell = new Vector2(x, y);
                    Event.current.Use();
                    EditorWindow.GetWindow<ProcGenRulesWindowUI>().Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    private static void HandleBlockingGrid(DecorationRuleUIModel gridRule)
    {
        var (_, gridWidth, gridHeight, _, _, blockGrid) = gridRule;

        if (_isBlockPaintActive && Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            var blockedCells = new System.Collections.Generic.List<Vector2>();
            for (int iy = 0; iy < gridHeight; iy++)
                for (int jx = 0; jx < gridWidth; jx++)
                    if (blockGrid[iy, jx])
                        blockedCells.Add(new Vector2(jx, iy));
            gridRule.PostSpawnBlockedCells = blockedCells.ToArray();

            _isBlockPaintActive = false;
        }

        for (int y = 0; y < gridHeight; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                GUI.backgroundColor = blockGrid[y, x] ? Color.red : Color.black;
                GUILayout.Box("", GUILayout.Width(ProcGenRulesWindowConstants.CELL_LENGTH),
                                  GUILayout.Height(ProcGenRulesWindowConstants.CELL_LENGTH));
                GUI.backgroundColor = Color.white;

                Rect cellRect = GUILayoutUtility.GetLastRect();
                bool mouseOver = cellRect.Contains(Event.current.mousePosition);

                if (mouseOver && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    _isBlockPaintActive = true;
                    _blockPaintTargetValue = !blockGrid[y, x];
                    if (blockGrid[y, x] != _blockPaintTargetValue)
                    {
                        blockGrid[y, x] = _blockPaintTargetValue;
                        Event.current.Use();
                        EditorWindow.GetWindow<ProcGenRulesWindowUI>().Repaint();
                    }
                }
                else if (_isBlockPaintActive && mouseOver && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    if (blockGrid[y, x] != _blockPaintTargetValue)
                    {
                        blockGrid[y, x] = _blockPaintTargetValue;
                        Event.current.Use();
                        EditorWindow.GetWindow<ProcGenRulesWindowUI>().Repaint();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
    }


    private static void HandleSpawnedPrefabField(DecorationRuleUIModel gridRule)
    {
        GUILayout.BeginHorizontal();
        gridRule.Prefab =
            (GameObject)EditorGUILayout.ObjectField(
                "Spawned Prefab",
                gridRule.Prefab,
                typeof(GameObject),
                false);
        GUILayout.EndHorizontal();
    }

    private static void HandleSpawnScaleField(DecorationRuleUIModel gridRule)
    {
        GUILayout.BeginHorizontal();
        gridRule.SpawnScale =
            EditorGUILayout.Vector3Field(
                "Spawn Scale",
                gridRule.SpawnScale);
        GUILayout.EndHorizontal();
    }

    private static void HandleSpawnRotationField(DecorationRuleUIModel gridRule)
    {
        GUILayout.BeginHorizontal();
        gridRule.SpawnRotation =
            EditorGUILayout.Vector3Field(
                "Spawn Rotation",
                gridRule.SpawnRotation);
        GUILayout.EndHorizontal();
    }

    private static void HandleSpawnOffsetField(DecorationRuleUIModel gridRule)
    {
        GUILayout.BeginHorizontal();
        gridRule.SpawnPositionOffset =
            EditorGUILayout.Vector3Field(
                "Spawn Offset",
                gridRule.SpawnPositionOffset);
        GUILayout.EndHorizontal();
    }

    private static void HandleDeleteButton(Action onDeleteClicked)
    {
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete Rule"))
        {
            onDeleteClicked?.Invoke();
        }
        GUILayout.EndHorizontal();
    }
}