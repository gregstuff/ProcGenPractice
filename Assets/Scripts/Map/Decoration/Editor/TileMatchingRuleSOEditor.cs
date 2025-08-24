#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TileMatchingRuleSO))]
public class TileMatchingRuleSOEditor : Editor
{
    SerializedProperty kindProp;
    SerializedProperty choiceProp;

    void OnEnable()
    {
        kindProp = serializedObject.FindProperty("kind");
        choiceProp = serializedObject.FindProperty("_choice");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 1) Show the selection dropdown
        EditorGUILayout.PropertyField(kindProp);

        // If the enum changed, ensure the backing choice type matches
        if (serializedObject.ApplyModifiedProperties())
        {
            foreach (var t in targets)
            {
                var cfg = (TileMatchingRuleSO)t;
                cfg.GetType().GetMethod("OnValidate",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                   ?.Invoke(cfg, null);
                EditorUtility.SetDirty(cfg);
            }
        }

        serializedObject.Update();

        // 2) Draw the fields of the selected class (OptionA/OptionB)
        EditorGUILayout.PropertyField(choiceProp, includeChildren: true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
