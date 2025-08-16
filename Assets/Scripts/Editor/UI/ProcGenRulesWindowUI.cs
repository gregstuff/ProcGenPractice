using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProcGenRulesWindowUI : EditorWindow
{
    private List<GridRule> rules = new List<GridRule>();
    private Vector2 scrollPosition;

    private TilePalette _tilePalette;
    private RuleResizeHandlerUI _resizeHandler;

    private void OnEnable()
    {
        _resizeHandler = new RuleResizeHandlerUI();
        _tilePalette = new TilePalette();
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
                 ()=>HandleDeleteButtonClicked(index),
                 (mousePos) =>
                 {
                     var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;
                     _resizeHandler.StartResize(index, mousePos, gridWidth, gridHeight);
                 }
             );
            _resizeHandler.HandleResizeEvents(gridRule, Repaint);
        }

        GUILayout.EndScrollView();
    }

    private void HandleAddButtonClicked()
    {
        rules.Add(new GridRule());
    }

    private void HandleDeleteButtonClicked(int index)
    {
        rules.RemoveAt(index);
    }
}