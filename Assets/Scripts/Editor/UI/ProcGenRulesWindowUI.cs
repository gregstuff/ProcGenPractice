using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ProcGenRulesWindowUI : EditorWindow
{
    // ---------------------------
    // Persistent selection state
    // ---------------------------
    private const string PrefKeyRuleSetGuid = "ProcGenRulesWindowUI_SelectedRuleSetGUID";
    private bool _lockRuleSetSelection;

    // ---------------------------
    // Core editor state
    // ---------------------------
    private readonly List<DecorationRuleUIModel> rules = new List<DecorationRuleUIModel>();
    private readonly List<bool> isRuleCollapsed = new List<bool>();
    private Vector2 scrollPosition;
    private TilePaletteUIModel _tilePalette;
    private RuleResizeHandlerUI _resizeHandler;
    private TileMatchingRuleSetSO _selectedTileMatchingRuleSet;

    // ---------------------------
    // Entry point
    // ---------------------------
    [MenuItem("Window/Proc Gen Rules")]
    public static void ShowWindow()
    {
        var window = GetWindow<ProcGenRulesWindowUI>("Proc Gen Rules Editor");
        window.Init();
    }

    private void OnEnable()
    {
        // Ensure state is valid after domain reloads or re-open
        if (_resizeHandler == null)
            _resizeHandler = new RuleResizeHandlerUI();

        if (_selectedTileMatchingRuleSet == null)
            LoadPersistedRuleSet();

        if (_tilePalette == null && _selectedTileMatchingRuleSet != null)
            _tilePalette = new TilePaletteUIModel(_selectedTileMatchingRuleSet);
    }

    private void Init()
    {
        _resizeHandler = new RuleResizeHandlerUI();
        LoadPersistedRuleSet();

        if(_selectedTileMatchingRuleSet!=null) _tilePalette = new TilePaletteUIModel(_selectedTileMatchingRuleSet);

        isRuleCollapsed.Clear();
        for (int i = 0; i < rules.Count; i++)
            isRuleCollapsed.Add(true);
    }

    // ---------------------------
    // GUI
    // ---------------------------
    private void OnGUI()
    {
        HeaderRuleSetSelectorGUI();
        if (_selectedTileMatchingRuleSet == null)
            return; // block the rest of the UI until a set is chosen

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
                (mousePos, rule) => _resizeHandler.StartResize(index, mousePos, gridWidth, gridHeight, rule),
                isRuleCollapsed[index],
                (isCollapsed) => isRuleCollapsed[index] = isCollapsed
            );

            _resizeHandler.HandleResizeEvents(gridRule, Repaint);
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// Toolbar UI to select/lock the TileMatchingRuleSetSO.
    /// Blocks rest of the UI until a set is chosen.
    /// </summary>
    private void HeaderRuleSetSelectorGUI()
    {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            // Typed ObjectField uses Unity's asset picker + drag & drop
            EditorGUI.BeginDisabledGroup(_lockRuleSetSelection && rules.Count > 0);
            var newSet = (TileMatchingRuleSetSO)EditorGUILayout.ObjectField(
                _selectedTileMatchingRuleSet, typeof(TileMatchingRuleSetSO), false, GUILayout.MinWidth(240));
            EditorGUI.EndDisabledGroup();

            if (newSet != _selectedTileMatchingRuleSet)
            {
                TrySetRuleSet(newSet);
            }

            // Optional quick dropdown of all TileMatchingRuleSetSO assets
            if (GUILayout.Button("Select…", EditorStyles.toolbarDropDown, GUILayout.Width(70)))
            {
                var guids = AssetDatabase.FindAssets("t:TileMatchingRuleSetSO");
                var assets = guids
                    .Select(g => AssetDatabase.GUIDToAssetPath(g))
                    .Select(p => AssetDatabase.LoadAssetAtPath<TileMatchingRuleSetSO>(p))
                    .Where(a => a != null)
                    .ToList();

                var menu = new GenericMenu();
                if (assets.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No assets found"));
                }
                else
                {
                    foreach (var a in assets)
                    {
                        var captured = a;
                        menu.AddItem(new GUIContent(a.name), captured == _selectedTileMatchingRuleSet, () =>
                        {
                            TrySetRuleSet(captured);
                        });
                    }
                }

                var rect = new Rect(Event.current.mousePosition, Vector2.zero);
                menu.DropDown(rect);
            }

            GUILayout.FlexibleSpace();

            // Lock toggle once rules exist (prevents accidental changes)
            using (new EditorGUI.DisabledScope(rules.Count == 0))
            {
                _lockRuleSetSelection = GUILayout.Toggle(
                    _lockRuleSetSelection, new GUIContent("Lock"), EditorStyles.toolbarButton);
            }
        }

        if (_selectedTileMatchingRuleSet == null)
        {
            EditorGUILayout.HelpBox("Select a Tile Matching Rule Set to begin.", MessageType.Info);
        }
        else
        {
            _tilePalette = new TilePaletteUIModel(_selectedTileMatchingRuleSet);
        }

    }

    /// <summary>
    /// Guarded setter that confirms clearing rules if switching sets.
    /// Rebuilds palette, persists selection, repaints.
    /// </summary>
    private void TrySetRuleSet(TileMatchingRuleSetSO newSet)
    {
        if (newSet == _selectedTileMatchingRuleSet)
            return;

        if (rules.Count > 0)
        {
            var ok = EditorUtility.DisplayDialog(
                "Change Rule Set?",
                "Changing the Tile Matching Rule Set may invalidate existing rules. " +
                "Switching will clear current rules.",
                "Switch & Clear", "Cancel");

            if (!ok) return;

            rules.Clear();
            isRuleCollapsed.Clear();
        }

        _selectedTileMatchingRuleSet = newSet;
        PersistRuleSet(newSet);

        _tilePalette = new TilePaletteUIModel(_selectedTileMatchingRuleSet);
        Repaint();
    }

    // ---------------------------
    // Persistence helpers
    // ---------------------------
    private void LoadPersistedRuleSet()
    {
        var guid = EditorPrefs.GetString(PrefKeyRuleSetGuid, "");
        if (string.IsNullOrEmpty(guid)) return;

        var path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path)) return;

        _selectedTileMatchingRuleSet = AssetDatabase.LoadAssetAtPath<TileMatchingRuleSetSO>(path);
    }

    private void PersistRuleSet(TileMatchingRuleSetSO so)
    {
        if (so != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(so, out string guid, out long _))
            EditorPrefs.SetString(PrefKeyRuleSetGuid, guid);
        else
            EditorPrefs.DeleteKey(PrefKeyRuleSetGuid);
    }

    // ---------------------------
    // List actions
    // ---------------------------
    private void HandleAddButtonClicked()
    {
        rules.Add(new DecorationRuleUIModel());
        isRuleCollapsed.Add(false);
    }

    private void HandleDeleteButtonClicked(int index)
    {
        if (index < 0 || index >= rules.Count) return;
        rules.RemoveAt(index);
        if (index < isRuleCollapsed.Count) isRuleCollapsed.RemoveAt(index);
    }

    // ---------------------------
    // Save / Load
    // ---------------------------
    private void HandleSaveButtonClicked()
    {
        string defaultFolder = "Assets/Resources/SO/Map/DecorationRulesets/";
        string path =
            EditorUtility.SaveFilePanelInProject(
                "Save Rule Set",
                "NewRuleSet",
                "asset",
                "Choose save location",
                defaultFolder);

        if (string.IsNullOrEmpty(path))
            return;

        if (InvalidSave(out var validationMessage))
        {
            Debug.LogError(validationMessage);
            return;
        }

        // Create and save ruleset asset (assumes your factory creates the asset at 'path')
        DecorationRulesetSO.Construct(path, rules.ToArray(), _selectedTileMatchingRuleSet);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Saved DecorationRulesetSO: {path}");
    }

    private void HandleLoadButtonClicked()
    {
        string defaultFolder = "Assets/Resources/SO/Map/DecorationRulesets/";
        string path = EditorUtility.OpenFilePanel("Load Rule Set", defaultFolder, "asset");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("No file selected.");
            return;
        }

        string assetPath = path.StartsWith(Application.dataPath)
            ? "Assets" + path.Substring(Application.dataPath.Length)
            : null;

        if (assetPath == null)
        {
            Debug.LogError($"Selected file is not in Assets: {path}");
            return;
        }

        var loaded = AssetDatabase.LoadAssetAtPath<DecorationRulesetSO>(assetPath);
        if (loaded == null)
        {
            Debug.LogError($"Failed to load ScriptableObject at path: {assetPath}");
            return;
        }

        // If the loaded ruleset references a different TileMatchingRuleSetSO, confirm switch
        // (Assumes DecorationRulesetSO has a .TileMatchingRuleSet reference)
        if (loaded.TileMatchingRuleSet != _selectedTileMatchingRuleSet)
        {
            var ok = EditorUtility.DisplayDialog(
                "Different Tile Rule Set Detected",
                $"The loaded ruleset uses '{loaded.TileMatchingRuleSet?.name ?? "None"}'. Switch to it?\n" +
                "Current in-memory rules will be replaced by the loaded rules.",
                "Switch", "Cancel");
            if (!ok) return;

            TrySetRuleSet(loaded.TileMatchingRuleSet);
        }

        var uiRules = loaded.DecorationRules.Select(DecorationRuleUIModel.FromModel).ToList();
        InitRules(uiRules);
        Repaint();

        Debug.Log($"Loaded DecorationRulesetSO: {assetPath}");
    }

    private bool InvalidSave(out string message)
    {
        message = null;

        if (_selectedTileMatchingRuleSet == null)
        {
            message = "No Tile Matching Rule Set selected.";
            return true;
        }

        if (rules.Count == 0)
        {
            message = "No rules!";
            return true;
        }

        var invalidRules = rules.Where(rule =>
            rule.Prefab == null 
            || !AssetDatabase.Contains(rule.Prefab) 
            || rule.MatchingPattern == null 
            || rule.PostSpawnBlockedCells == null).ToList();

        if (invalidRules.Any())
        {
            message = $"Invalid rule(s): {string.Join(", ", invalidRules.Select(rule => rule.Name))}. " +
                      $"Ensure prefabs are valid and MatchingPattern is initialized.";
            return true;
        }

        return false;
    }

    private void InitRules(List<DecorationRuleUIModel> uiRules)
    {
        rules.Clear();
        rules.AddRange(uiRules);

        isRuleCollapsed.Clear();
        for (int i = 0; i < rules.Count; i++)
            isRuleCollapsed.Add(true);
    }
}
