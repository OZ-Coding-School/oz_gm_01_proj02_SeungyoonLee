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

        Rect checkBoxRect = new Rect(position.x, position.y, checkBoxSize, position.height);
        Rect fieldRect =
            new Rect(
                position.x + checkBoxSize,
                position.y,
                position.width - checkBoxSize,
                position.height);

        hasValueProperty.boolValue = EditorGUI.Toggle(checkBoxRect, hasValueProperty.boolValue);
        if (hasValueProperty.boolValue) EditorGUI.PropertyField(fieldRect, valueProperty, label, true);
        else EditorGUI.LabelField(fieldRect, label.text, "<null>");
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
