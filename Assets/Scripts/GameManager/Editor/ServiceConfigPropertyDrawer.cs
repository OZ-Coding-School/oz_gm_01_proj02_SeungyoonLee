using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LunarCube.GameManager.IServiceConfig))]
public class ServiceConfigPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        string displayName = ObjectNames.NicifyVariableName(property.managedReferenceFullTypename.Split(' ', '.').Last());
        EditorGUI.PropertyField(rect, property, new GUIContent(displayName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
