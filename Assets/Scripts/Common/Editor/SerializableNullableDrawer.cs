using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LunarCube.SerializableNullable<>), true)]
public class SerializableNullableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty hasValueProperty = property.FindPropertyRelative("hasValue");
        SerializedProperty valueProperty = property.FindPropertyRelative("value");

        float checkBoxSize = EditorGUIUtility.singleLineHeight;

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        Rect checkBoxPosition = new Rect(contentPosition.x,
                                         contentPosition.y,
                                         checkBoxSize,
                                         contentPosition.height);
        Rect fieldPosition =
            new Rect(
                contentPosition.x + checkBoxSize,
                contentPosition.y,
                contentPosition.width - checkBoxSize,
                contentPosition.height);


        int originalIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        hasValueProperty.boolValue = EditorGUI.Toggle(checkBoxPosition, hasValueProperty.boolValue);
        if (hasValueProperty.boolValue) EditorGUI.PropertyField(fieldPosition, valueProperty, GUIContent.none, true);
        else EditorGUI.LabelField(fieldPosition, "<null>");
        EditorGUI.indentLevel = originalIndentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var hasValueProperty = property.FindPropertyRelative("hasValue");
        return
            hasValueProperty.boolValue
                ? EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), label, true)
                : EditorGUIUtility.singleLineHeight;
    }
}
