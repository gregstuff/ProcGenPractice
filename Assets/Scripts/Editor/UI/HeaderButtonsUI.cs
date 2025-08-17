using System;
using UnityEngine;

public class HeaderButtonsUI
{
    private static readonly GUIStyle PANEL_STYLE = new GUIStyle("box") { margin = new RectOffset(5, 5, 5, 5), padding = new RectOffset(10, 10, 10, 10) };
    private static readonly GUIStyle BUTTON_STYLE = new GUIStyle("button") { fixedWidth = 120f, fixedHeight = 30f, alignment = TextAnchor.MiddleCenter };

    private static readonly int BUTTON_RECT_X = 10;
    private static readonly int BUTTON_RECT_Y = 10;
    private static readonly int BUTTON_RECT_WIDTH = 120;
    private static readonly int BUTTON_RECT_HEIGHT = 30;

    public static HeaderButtonsUI Construct(
        Action onAddNewRuleClicked,
        Action onSaveClicked,
        Action onLoadClicked,
        Rect position)
    {
        GUI.BeginGroup(position, PANEL_STYLE);

        if (GUI.Button(GetButtonRectForIndex(0), "Add New Rule", BUTTON_STYLE))
        {
            onAddNewRuleClicked?.Invoke();
        }

        if (GUI.Button(GetButtonRectForIndex(1), "Save Rules", BUTTON_STYLE))
        {
            onSaveClicked?.Invoke();
        }

        if (GUI.Button(GetButtonRectForIndex(2), "Load Rules", BUTTON_STYLE))
        {
            onLoadClicked?.Invoke();
        }

        GUI.EndGroup();
        return new HeaderButtonsUI();
    }

    private static Rect GetButtonRectForIndex(int index)
    {
        return new Rect()
        {
            x = BUTTON_RECT_X,
            y = BUTTON_RECT_Y + (BUTTON_RECT_HEIGHT * index),
            width = BUTTON_RECT_WIDTH,
            height = BUTTON_RECT_HEIGHT
        };
    }
}