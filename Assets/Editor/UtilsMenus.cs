using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class UtilsMenus
{
    /// <summary>
    /// 右键创建Lua脚本，其实就是个文本文件后缀是txt，因为Unity不支持Lua文件
    /// </summary>
    [MenuItem("Assets/Create/Lua Script", false, 80)]
    public static void CreateNewLua()
    {
        string strFilePath = "Assets/Editor/Templates/LuaTemplate.lua.txt";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MyDoCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/New Lua.lua.txt",
        null,
       strFilePath);
    }

    /// <summary>
    /// Luas to text.
    /// </summary>
    [MenuItem("UtilsTools/Change Lua Type #c")]
    static void LuaToTxt()
    {
        string[] select = Selection.assetGUIDs;
        for (int i = 0; i < select.Length; i++)
        {
            string strTempPath = AssetDatabase.GUIDToAssetPath(select[i]);
            FileInfo tempFile = new FileInfo(strTempPath);
            string strFileExtension = Path.GetExtension(strTempPath).TrimStart('.');
            strTempPath = strTempPath.Substring(0, strTempPath.IndexOf('.'));
            strTempPath += strFileExtension == "lua" ? ".txt" : ".lua";
            tempFile.MoveTo(strTempPath);
        }
    }

    #region Private Methods
    static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
    #endregion
}

class MyDoCreateScriptAsset : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
        ProjectWindowUtil.ShowCreatedAsset(o);
    }

    internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
    {
        string fullPath = Path.GetFullPath(pathName);
        StreamReader streamReader = new StreamReader(resourceFile);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
        text = Regex.Replace(text, "#NAME#", fileNameWithoutExtension);
        bool encoderShouldEmitUTF8Identifier = true;
        bool throwOnInvalidBytes = false;
        UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
        bool append = false;
        StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
        streamWriter.Write(text);
        streamWriter.Close();
        AssetDatabase.ImportAsset(pathName);
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
}

