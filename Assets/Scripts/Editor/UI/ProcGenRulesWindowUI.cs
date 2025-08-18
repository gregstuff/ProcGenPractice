using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ProcGenRulesWindowUI : EditorWindow
{
    private List<DecorationRuleUIModel> rules = new List<DecorationRuleUIModel>();
    private List<bool> isRuleCollapsed = new List<bool>();
    private Vector2 scrollPosition;
    private TilePaletteUIModel _tilePalette;
    private RuleResizeHandlerUI _resizeHandler;

    [MenuItem("Window/Proc Gen Rules")]
    public static void ShowWindow()
    {
        Debug.Log("Show window!");
        var window = GetWindow<ProcGenRulesWindowUI>("Proc Gen Rules Editor");
        window.Init();
    }

    private void Init()
    {
        _resizeHandler = new RuleResizeHandlerUI();
        _tilePalette = new TilePaletteUIModel();

        isRuleCollapsed.Clear();
        for (int i = 0; i < rules.Count; i++)
        {
            isRuleCollapsed.Add(true);
        }
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
            var (name, gridWidth, gridHeight, gridPattern, spawnLocationGrid, blockLocationGrid) = gridRule;
            RuleCardUI.Construct(
                gridRule,
                _tilePalette,
                () => HandleDeleteButtonClicked(index),
                (mousePos, gridRule) =>
                {
                    _resizeHandler.StartResize(index, mousePos, gridWidth, gridHeight, gridRule);
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
            return;
        }

        DecorationRulesetSO.Construct(path, rules.ToArray());
    }

    private bool InvalidSave(out string message)
    {
        message = null;

        if (rules.Count == 0)
        {
            message = "No rules!";
            return true;
        }

        var invalidRules = rules.Where(rule =>
                rule.Prefab == null || !AssetDatabase.Contains(rule.Prefab) ||
                rule.MatchingPattern == null);
        if (invalidRules.Any())
        {
            message = $"Invalid rule(s): {string.Join(", ", invalidRules.Select(rule => rule.Name))}. Ensure prefabs are valid and MatchingPattern is initialized.";
            return true;
        }

        return false;
    }

    private void HandleLoadButtonClicked()
    {
        string path = EditorUtility.OpenFilePanel(
            "Load Rule Set",
            "Assets/Resources/SO/Map/DecorationRulesets/",
            "asset");
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No file selected.");
            return;
        }
        string assetPath = path;
        if (path.StartsWith(Application.dataPath))
        {
            assetPath = "Assets" + path.Substring(Application.dataPath.Length);
        }
        else
        {
            Debug.LogError($"Selected file is not in Assets: {path}");
            return;
        }
        DecorationRulesetSO loadedRuleset = AssetDatabase.LoadAssetAtPath<DecorationRulesetSO>(assetPath);
        if (loadedRuleset == null)
        {
            Debug.LogError($"Failed to load ScriptableObject at path: {assetPath}");
            return;
        }
        var uiRules = loadedRuleset.DecorationRules.Select(DecorationRuleUIModel.FromModel).ToList();
        InitRules(uiRules);
    }

    private void InitRules(List<DecorationRuleUIModel> uiRules)
    {
        rules.Clear();
        rules.AddRange(uiRules);
        rules.ForEach(rule => isRuleCollapsed.Add(true));
    }

    private void HandleDeleteButtonClicked(int index)
    {
        rules.RemoveAt(index);
        isRuleCollapsed.RemoveAt(index); 
    }
}