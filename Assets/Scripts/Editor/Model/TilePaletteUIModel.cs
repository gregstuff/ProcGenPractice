
using DungeonGeneration.Map.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilePaletteUIModel
{

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
        Color.black,
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

    private static Dictionary<KeyCode, TileType> _keyToTileType = new Dictionary<KeyCode, TileType>();
    private static Dictionary<TileType, Color> _tileTypeToColor = new Dictionary<TileType, Color>();
    private static Dictionary<TileType, KeyCode> _tileTypeToKeyCode = new Dictionary<TileType, KeyCode>();

    public TilePaletteUIModel()
    {
        Init();
    }

    private void Init()
    {
        _keyToTileType = new Dictionary<KeyCode, TileType>();
        _tileTypeToColor = new Dictionary<TileType, Color>();
        _tileTypeToKeyCode = new Dictionary<TileType, KeyCode>();

        var tileTypes = (TileType[]) System.Enum.GetValues(typeof(TileType));

        if (tileTypes.Length > 10) throw new Exception("Too many tile types!!! Please refactor the keycode / color handling");

        foreach (var tileType in tileTypes)
        {
            int index = Array.IndexOf(tileTypes, tileType);
            _keyToTileType.Add(_tilePaletteKeyCodes[index], tileType);
            _tileTypeToColor.Add(tileType, _tileColors[index]);
        }

        _tileTypeToKeyCode = _keyToTileType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    }

    public TileType? GetTileTypeForKeyCode(KeyCode candidate)
    {
        return _keyToTileType.TryGetValue(candidate, out var tileType) ? tileType : null;
    }

    public Color? GetColorForTileType(TileType tileType)
    {
        return _tileTypeToColor.TryGetValue(tileType, out var color) ? color : null;
    }

    public KeyCode? GetKeyCodeForTileType(TileType tileType)
    {
        return _tileTypeToKeyCode.TryGetValue(tileType, out var keyCode) ? keyCode : null;
    }

}
