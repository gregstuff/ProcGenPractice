using System;
using UnityEngine;

public class HeaderButtonsUI
{
    private static readonly GUIStyle PANEL_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5), padding = new RectOffset(10, 10, 10, 10) };
    private static readonly GUIStyle BUTTON_STYLE = new GUIStyle("button") { fixedWidth = 120f, fixedHeight = 30f, alignment = TextAnchor.MiddleCenter };

    public static HeaderButtonsUI Construct(Action onAddNewRuleClicked, Rect position)
    {
        GUI.BeginGroup(position, PANEL_STYLE);
        // Draw button at (10, 10) relative to the group's top-left corner
        if (GUI.Button(new Rect(10, 10, 120, 30), "Add New Rule", BUTTON_STYLE))
        {
            onAddNewRuleClicked?.Invoke();
        }
        GUI.EndGroup();
        return new HeaderButtonsUI();
    }
}