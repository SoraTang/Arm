#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for ArmCore to expose the Detached property in the Inspector
/// and call its setter when toggled at runtime or in edit mode.
/// </summary>
[CustomEditor(typeof(ArmCore))]
public class ArmCoreEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var arm = (ArmCore)target;

        // Toggle for Detached property
        EditorGUI.BeginChangeCheck();
        bool newState = EditorGUILayout.Toggle("Detached", arm.Detached);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(arm, "Toggle Detached");
            arm.Detached = newState;
            EditorUtility.SetDirty(arm);
        }

        // Draw remaining default fields, excluding the private detachedState
        serializedObject.Update();
        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.name == "detachedState")
                continue;
            EditorGUILayout.PropertyField(prop, true);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif