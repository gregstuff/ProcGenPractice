using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePaletteUIModel
{

    public TileMatchingRuleSetSO TileMatchingRuleSet;

    private static readonly KeyCode[] _tilePaletteKeyCodes =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0
    };

    private static readonly Color[] _tileColors =
    { 
        Color.red,
        Color.green,
        Color.blue,
        Color.magenta,
        Color.cyan,
        Color.yellow,
        Color.gray,
        new Color(255,165,0), //orange
        new Color(255,99,71) //pinkish
    };

    private static Dictionary<KeyCode, TileMatchingRuleSO> _keyToTileType = new Dictionary<KeyCode, TileMatchingRuleSO>();
    private static Dictionary<TileMatchingRuleSO, Color> _tileTypeToColor = new Dictionary<TileMatchingRuleSO, Color>();
    private static Dictionary<TileMatchingRuleSO, KeyCode> _tileTypeToKeyCode = new Dictionary<TileMatchingRuleSO, KeyCode>();

    public TilePaletteUIModel(TileMatchingRuleSetSO tileMatchingRuleSet)
    {
        TileMatchingRuleSet = tileMatchingRuleSet;
        Init();
    }

    private void Init()
    {
        _keyToTileType = new Dictionary<KeyCode, TileMatchingRuleSO>();
        _tileTypeToColor = new Dictionary<TileMatchingRuleSO, Color>();
        _tileTypeToKeyCode = new Dictionary<TileMatchingRuleSO, KeyCode>();

        var tileMatchingRules = TileMatchingRuleSet.TileMatchingRules;

        if (tileMatchingRules.Length > 10) throw new Exception("Too many tile types!!! Please refactor the keycode / color handling");

        foreach (var tileType in tileMatchingRules)
        {
            int index = Array.IndexOf(tileMatchingRules, tileType);
            _keyToTileType.Add(_tilePaletteKeyCodes[index], tileType);
            _tileTypeToColor.Add(tileType, _tileColors[index]);
        }

        _tileTypeToKeyCode = _keyToTileType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    }

    public TileMatchingRuleSO? GetTileTypeForKeyCode(KeyCode candidate)
    {
        return _keyToTileType.TryGetValue(candidate, out var tileType) ? tileType : null;
    }

    public Color? GetColorForTileType(TileMatchingRuleSO tileType)
    {
        if (tileType == null) return Color.black;
        return _tileTypeToColor.TryGetValue(tileType, out var color) ? color : null;
    }

    public KeyCode? GetKeyCodeForTileType(TileMatchingRuleSO tileType)
    {
        return _tileTypeToKeyCode.TryGetValue(tileType, out var keyCode) ? keyCode : null;
    }

}
