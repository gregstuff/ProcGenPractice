using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ProcGenRulesWindowUI : EditorWindow
{
    private List<DecorationRuleUIModel> rules = new List<DecorationRuleUIModel>();
    private List<bool> isRuleCollapsed = new List<bool>();
    private Vector2 scrollPosition;
    private TilePaletteUIModel _tilePalette;
    private RuleResizeHandlerUI _resizeHandler;

    private void OnEnable()
    {
        _resizeHandler = new RuleResizeHandlerUI();
        _tilePalette = new TilePaletteUIModel();

        isRuleCollapsed.Clear(); 
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
        StickyUIPanel.Construct(
            _tilePalette,
            HandleAddButtonClicked,
            HandleSaveButtonClicked,
            HandleLoadButtonClicked,
            position);

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
        rules.Add(new DecorationRuleUIModel());
        isRuleCollapsed.Add(false);
    }

    private void HandleSaveButtonClicked()
    {
        string path = 
            EditorUtility.SaveFilePanelInProject(
                "Save Rule Set", 
                "NewRuleSet", 
                "asset", 
                "Choose save location",
                "Assets/Resources/SO/Map/DecorationRulesets/");

        if (InvalidSave(out var validaitonMessage))
        {
            Debug.Log(validaitonMessage);
        }

        DecorationRulesetSO.Construct(path, rules.ToArray());
    }

    private bool InvalidSave(out string message)
    {
        message = "hello";
        return false;
    } 

    private void HandleLoadButtonClicked()
    {

    }

    private void HandleDeleteButtonClicked(int index)
    {
        rules.RemoveAt(index);
        isRuleCollapsed.RemoveAt(index); 
    }
}