using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PartVariantsImporter : EditorWindow
{
    private PartVariants partVariantsSO;
    private string folderPath = "Assets/";

    [MenuItem("Tools/PartVariants Importer")]
    public static void ShowWindow()
    {
        GetWindow<PartVariantsImporter>("PartVariants Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Prefab Paths into PartVariants", EditorStyles.boldLabel);

        partVariantsSO = (PartVariants)EditorGUILayout.ObjectField("PartVariants SO", partVariantsSO, typeof(PartVariants), false);
        folderPath = EditorGUILayout.TextField("Folder Path (from Assets/)", folderPath);

        if (GUILayout.Button("Import Prefabs"))
        {
            if (partVariantsSO == null)
            {
                Debug.LogError("Please assign a PartVariants ScriptableObject first!");
                return;
            }

            ImportPrefabs();
        }
    }

    private void ImportPrefabs()
    {
        string fullPath = Path.Combine(Application.dataPath, folderPath.Replace("Assets/", ""));
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Folder path does not exist: {fullPath}");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        List<string> prefabPaths = new List<string>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Remove "Assets/Resources/" from the start, and ".prefab" from the end
            if (assetPath.StartsWith("Assets/Resources/"))
            {
                assetPath = assetPath.Substring("Assets/Resources/".Length);
            }
            else
            {
                Debug.LogWarning($"Prefab not in Resources folder: {assetPath}");
                continue; // skip prefabs not inside Resources
            }

            if (assetPath.EndsWith(".prefab"))
            {
                assetPath = assetPath.Substring(0, assetPath.Length - ".prefab".Length);
            }

            prefabPaths.Add(assetPath);
        }


        partVariantsSO.prefabPaths = prefabPaths;
        EditorUtility.SetDirty(partVariantsSO);
        AssetDatabase.SaveAssets();

        Debug.Log($"Imported {prefabPaths.Count} prefabs into {partVariantsSO.name}");
    }
}
