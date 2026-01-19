using Lua.Unity;
using System.Collections.Generic;
using System.IO;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class LuaAssetHandlerUtility
{
    [MenuItem("Assets/Create/Scripting/Empty Lua Script", false)]
    public static void CreateTextAsset()
    {
        Texture2D icon = AssetPreview.GetMiniTypeThumbnail(typeof(LuaAsset));

        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
            0,
            ScriptableObject.CreateInstance<EndNameEditForLuaAssetAction>(),
            "NewLuaScript.lua",
            icon,
            null);
    }

    class EndNameEditForLuaAssetAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            File.WriteAllText(pathName, "\n");
            AssetDatabase.ImportAsset(pathName);

            Object obj = AssetDatabase.LoadAssetAtPath<TextAsset>(pathName);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }

    //[OnOpenAsset]
    //public static bool OnOpenLuaAsset(EntityId entityId, int line, int column)
    //{
    //    string assetPath = Path.GetFullPath(AssetDatabase.GetAssetPath(entityId));
    //    if (!assetPath.EndsWith(".lua")) return false;
    //    Dictionary<string, string> pathMap = CodeEditor.Editor.GetFoundScriptEditorPaths();
    //    if (!pathMap.TryGetValue(CodeEditor.CurrentEditorPath, out string editorName)) return false;


    //    string arguments;
    //    if (editorName.Contains("Visual Studio Code"))  arguments = $"--reuse-window --goto \"{assetPath}\":{line}:{column}";
    //    else if (editorName.Contains("Visual Studio"))  arguments = $"/Edit \"{assetPath}\" /Command \"edit.goto {line + 1}\"";
    //    else if (editorName.Contains("Rider"))          arguments = $"--line {line} \"{assetPath}\"";
    //    else return false;

    //    try
    //    {
    //        System.Diagnostics.Process process = System.Diagnostics.Process.Start(CodeEditor.CurrentEditorPath, arguments);
    //        return null != process;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}
}
