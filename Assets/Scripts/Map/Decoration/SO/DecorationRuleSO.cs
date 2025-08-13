using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct PatternRow
{
    public PatternCellType[] RowValues;
}

public abstract class DecorationRuleSO : ScriptableObject
{

    [SerializeField] PatternRow[] PatternRows;
    [SerializeField] int PatternHeight;
    [SerializeField] int PatternWidth;

    private int _lastWidth;
    private int _lastHeight;

    private void OnValidate()
    {
        if (_lastHeight == PatternHeight && _lastWidth == PatternWidth) return;

        _lastWidth = PatternWidth;
        _lastHeight = PatternHeight;

        PatternRows = new PatternRow[PatternHeight];

        for (int i = 0; i < PatternHeight; ++i)
        {
            PatternRows[i].RowValues = new PatternCellType[PatternWidth];
        }
    }

}