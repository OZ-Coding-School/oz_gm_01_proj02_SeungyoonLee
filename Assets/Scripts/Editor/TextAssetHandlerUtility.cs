using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

public class TextAssetHandlerUtility
{
    [MenuItem("Assets/Create/Text", false, -215)]
    public static void CreateTextAsset()
    {
        Texture2D icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<EndNameEditForTextAssetAction>(),
            "NewTextAsset.txt",
            icon,
            null);
    }

    class EndNameEditForTextAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            File.WriteAllText(pathName, "\n");
            AssetDatabase.ImportAsset(pathName);

            Object obj = AssetDatabase.LoadAssetAtPath<TextAsset>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }
}
