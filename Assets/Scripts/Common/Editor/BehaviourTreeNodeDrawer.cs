using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LunarCube.BehaviourTree.INode), true)]
public class BehaviourTreeNodeDrawer : PropertyDrawer
{
    private string nullPlaceholder = "<Null>";

    private static List<Type> GetOrderedNodeTypeList()
    {
        return TypeCache.GetTypesDerivedFrom<LunarCube.BehaviourTree.INode>()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.IsSerializable)
            .OrderBy(t => GetCategory(t))
            .ThenBy(t => t.Name)
            .ToList();
    }

    private static string GetCategory(Type t)
    {
        if (typeof(LunarCube.BehaviourTree.Compositor).IsAssignableFrom(t)) return "1-Compositor";
        if (typeof(LunarCube.BehaviourTree.Modifier).IsAssignableFrom(t))   return "2-Modifier";
        return null;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
        float labelWidth = EditorGUIUtility.labelWidth;

        string typeName = property.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(typeName)) typeName = nullPlaceholder;
        else                                typeName = typeName.Split(' ', '.').Last();

        Rect labelPosition = new Rect(position.x,
                                      position.y,
                                      labelWidth,
                                      singleLineHeight);
        Rect dropdownPosition = new Rect(position.x + labelWidth,
                                         position.y,
                                         position.width - labelWidth,
                                         singleLineHeight);

        if (property.hasVisibleChildren)
        {
            property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label, true);
        }
        else
        {
            EditorGUI.LabelField(labelPosition, label);
        }

        if (GUI.Button(dropdownPosition, typeName, EditorStyles.popup))
        {
            ShowTypeMenu(property);
        }

        if (property.isExpanded && property.hasVisibleChildren)
        {
            EditorGUI.indentLevel++;

            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool isEnteringChildren = true;

            float y = position.y + singleLineHeight + standardVerticalSpacing;

            while (iterator.NextVisible(isEnteringChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                isEnteringChildren = false;
                float childHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect childPosition = new Rect(position.x, y, position.width, childHeight);

                EditorGUI.PropertyField(childPosition, iterator, true);

                y += childHeight + standardVerticalSpacing;
            }

            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

        float y = singleLineHeight;

        if (property.isExpanded && property.hasVisibleChildren)
        {
            SerializedProperty iterator = property.Copy();
            SerializedProperty endProperty = iterator.GetEndProperty();
            bool isEnteringChildren = true;

            while (iterator.NextVisible(isEnteringChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                isEnteringChildren = false;
                float childHeight = EditorGUI.GetPropertyHeight(iterator, true);
                y += childHeight + standardVerticalSpacing;
            }
            y += standardVerticalSpacing;
        }

        return y;
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        List<Type> types = GetOrderedNodeTypeList();

        GenericMenu menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("<Null>"),
            string.IsNullOrEmpty(property.managedReferenceFullTypename),
            () =>
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

        menu.AddSeparator("");

        foreach (Type type in types)
        {
            string category = GetCategory(type)?[2..] ?? "";
            string path = $"{category}{(!string.IsNullOrEmpty(category) ? "/" : "")}{type.Name}";

            menu.AddItem(
                new GUIContent(path),
                property.managedReferenceFullTypename.EndsWith(type.Name),
                () =>
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
        }

        menu.ShowAsContext();
    }
}
