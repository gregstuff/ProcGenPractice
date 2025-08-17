using UnityEditor;
using UnityEngine;

public class RuleResizeHandlerUI
{
    private int clickedRuleIndex = -1;
    private Vector2 dragStartPos;
    private int startWidth;
    private int startHeight;

    public RuleResizeHandlerUI()
    {

    }

    public void StartResize(int index, Vector2 mousePosition, int width, int height)
    {
        clickedRuleIndex = index;
        dragStartPos = mousePosition;
        startWidth = width;
        startHeight = height;
    }

    public void HandleResizeEvents(DecorationRuleUIModel gridRule, System.Action repaintCallback)
    {
        if (clickedRuleIndex == -1 || GUIUtility.hotControl == 0)
            return;

        var (_, gridWidth, gridHeight, gridPattern, _, _) = gridRule;

        if (Event.current.type == EventType.MouseDrag)
        {
            Vector2 delta = Event.current.mousePosition - dragStartPos;
            int addCols = (int)(delta.x / ProcGenRulesWindowConstants.CELL_LENGTH);
            int addRows = (int)(delta.y / ProcGenRulesWindowConstants.CELL_LENGTH);
            int newWidth = Mathf.Max(3, startWidth + addCols);
            int newHeight = Mathf.Max(3, startHeight + addRows);

            if (newWidth != gridWidth || newHeight != gridHeight)
            {
                gridRule.ResizeGrid(newHeight, newWidth);
                repaintCallback?.Invoke();
            }
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            GUIUtility.hotControl = 0;
            clickedRuleIndex = -1;
            Event.current.Use();
        }
    }
}