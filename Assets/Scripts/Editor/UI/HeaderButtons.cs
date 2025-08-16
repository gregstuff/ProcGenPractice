using System;
using UnityEngine;

public class HeaderButtons
{

    public Action OnAddButtonClicked;

    public HeaderButtons()
    {

    }

    public static HeaderButtons Construct(Action onAddNewRuleClicked)
    {
        GUILayout.BeginHorizontal();
        var headerButtons = new HeaderButtons();

        if (GUILayout.Button("Add New Rule"))
        {
            onAddNewRuleClicked?.Invoke();
        }

        GUILayout.EndHorizontal();
        return headerButtons;
    }
}
