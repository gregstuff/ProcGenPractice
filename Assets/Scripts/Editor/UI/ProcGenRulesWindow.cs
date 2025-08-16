using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProcGenRulesWindow : EditorWindow
{
    private List<GridRule> rules = new List<GridRule>();
    private Vector2 scrollPosition;

    private TilePalette _tilePalette;
    private RuleResizeHandler _resizeHandler;

    private void OnEnable()
    {
        _resizeHandler = new RuleResizeHandler();
        _tilePalette = new TilePalette();
    }


    [MenuItem("Window/Proc Gen Rules")]
    public static void ShowWindow()
    {
        GetWindow<ProcGenRulesWindow>("Proc Gen Rules Editor");
    }

    private void OnGUI()
    {
        var headerButtons = HeaderButtons.Construct(HandleAddButtonClicked);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        for (int index = 0; index < rules.Count; index++)
        {
            var gridRule = rules[index];
            var (gridWidth, gridHeight, gridPattern, gridID) = gridRule;
            RuleCard.Construct(
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