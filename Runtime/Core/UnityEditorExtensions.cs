using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

public static class UnityEditorExtensions
{
#if UNITY_EDITOR
    /// <summary>
    /// Empieza automaticamente desde Assets
    /// </summary>
    /// <param name="folders"></param>
    public static string CreateFoldersRecursive(params string[] folders)
    {
        string currentPath = "Assets/";
        for (int i = 0; i < folders.Length; i++)
        {
            Debug.Log(currentPath);
            string parentFolder = currentPath.Remove(currentPath.Length - 1, 1);
            string subfolder = folders[i];
            if(!AssetDatabase.IsValidFolder(currentPath + subfolder))
                AssetDatabase.CreateFolder(parentFolder, subfolder);
            currentPath += $"{folders[i]}/";
        }
        AssetDatabase.Refresh();
        return currentPath;
    }
    
    public static T AssetFromGuid<T>(string assetGuid) where T : Object
    {
        string path = AssetDatabase.GUIDToAssetPath(assetGuid);
        return  AssetDatabase.LoadAssetAtPath<T>(path);
    }

    public static T FindAssetByType<T>() where T : UnityEngine.Object => FindAssetsByType<T>()[0];
    public static List<T> FindAssetsByType<T>() => FindAssetsByType<T>(typeof(T));
    public static List<T> FindAssetsByType<T>(Type type)
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{type}");
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            UnityEngine.Object[] found = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for (int index = 0; index < found.Length; index++)
                if (found[index] is T item && !assets.Contains(item))
                    assets.Add(item);
        }
        return assets;
    }

    public static T[] LoadFilesInFolder<T>(string folderPath, string pattern, SearchOption searchOption) where T : Object
    {
        string[] files = Directory.GetFiles(folderPath, pattern, searchOption);
        T[] results = new T[files.Length];
        for (var index = 0; index < files.Length; index++)
        {
            string file = files[index];
            string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            results[index] = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        return results;
    }
#endif
}