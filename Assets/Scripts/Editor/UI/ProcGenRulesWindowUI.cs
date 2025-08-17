using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProcGenRulesWindowUI : EditorWindow
{
    private List<GridRule> rules = new List<GridRule>();
    private List<bool> isRuleCollapsed = new List<bool>(); // Tracks collapsed state for each rule
    private Vector2 scrollPosition;
    private TilePalette _tilePalette;
    private RuleResizeHandlerUI _resizeHandler;

    private void OnEnable()
    {
        _resizeHandler = new RuleResizeHandlerUI();
        _tilePalette = new TilePalette();
        // Initialize all rules as collapsed (true)
        isRuleCollapsed.Clear(); // Ensure clean state
        for (int i = 0; i < rules.Count; i++)
        {
            isRuleCollapsed.Add(true);
        }
    }

    [MenuItem("Window/Proc Gen Rules")]
    public static void ShowWindow()
    {
        GetWindow<ProcGenRulesWindowUI>("Proc Gen Rules Editor");
    }

    private void OnGUI()
    {
        StickyUIPanel.Construct(_tilePalette, HandleAddButtonClicked, position);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int index = 0; index < rules.Count; index++)
        {
            var gridRule = rules[index];
            var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;
            RuleCardUI.Construct(
                gridRule,
                _tilePalette,
                () => HandleDeleteButtonClicked(index),
                (mousePos) =>
                {
                    var (w, h, pattern, id) = gridRule;
                    _resizeHandler.StartResize(index, mousePos, w, h);
                },
                isRuleCollapsed[index],
                (isCollapsed) => isRuleCollapsed[index] = isCollapsed
            );
            _resizeHandler.HandleResizeEvents(gridRule, Repaint);
        }
        GUILayout.EndScrollView();
    }

    private void HandleAddButtonClicked()
    {
        rules.Add(new GridRule());
        isRuleCollapsed.Add(false); // New rules are expanded by default
    }

    private void HandleDeleteButtonClicked(int index)
    {
        rules.RemoveAt(index);
        isRuleCollapsed.RemoveAt(index); // Remove corresponding collapsed state
    }
}