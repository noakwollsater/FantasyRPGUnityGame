using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PartVariantsImporter : EditorWindow
{
    private PartVariants partVariantsSO;
    private string folderPathLeft = "Assets/";
    private string folderPathRight = "Assets/";
    private bool isDualPath = false;

    public static void ShowWindow()
    {
        GetWindow<PartVariantsImporter>("PartVariants Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import Prefabs into PartVariants", EditorStyles.boldLabel);

        partVariantsSO = (PartVariants)EditorGUILayout.ObjectField("PartVariants SO", partVariantsSO, typeof(PartVariants), false);

        isDualPath = EditorGUILayout.Toggle("Dual Path (Left/Right)", isDualPath);

        if (isDualPath)
        {
            folderPathLeft = EditorGUILayout.TextField("Left Folder Path", folderPathLeft);
            folderPathRight = EditorGUILayout.TextField("Right Folder Path", folderPathRight);
        }
        else
        {
            folderPathLeft = EditorGUILayout.TextField("Single Folder Path", folderPathLeft);
        }

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
        partVariantsSO.variants.Clear(); // Clear old data first

        if (isDualPath)
        {
            string[] leftGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPathLeft });
            string[] rightGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPathRight });

            List<string> leftPaths = ProcessGuids(leftGuids);
            List<string> rightPaths = ProcessGuids(rightGuids);

            int pairCount = Mathf.Min(leftPaths.Count, rightPaths.Count);

            for (int i = 0; i < pairCount; i++)
            {
                PartVariants.VariantEntry entry = new PartVariants.VariantEntry
                {
                    isDualPath = true,
                    leftPath = leftPaths[i],
                    rightPath = rightPaths[i]
                };
                partVariantsSO.variants.Add(entry);
            }

            Debug.Log($"Imported {pairCount} left/right pairs into {partVariantsSO.name}");
        }
        else
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPathLeft });
            List<string> paths = ProcessGuids(guids);

            foreach (var path in paths)
            {
                PartVariants.VariantEntry entry = new PartVariants.VariantEntry
                {
                    isDualPath = false,
                    singlePath = path
                };
                partVariantsSO.variants.Add(entry);
            }

            Debug.Log($"Imported {paths.Count} single prefabs into {partVariantsSO.name}");
        }

        EditorUtility.SetDirty(partVariantsSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private List<string> ProcessGuids(string[] guids)
    {
        List<string> paths = new List<string>();

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (assetPath.StartsWith("Assets/Resources/"))
            {
                assetPath = assetPath.Substring("Assets/Resources/".Length);
            }
            else
            {
                Debug.LogWarning($"Prefab not in Resources folder: {assetPath}");
                continue;
            }

            if (assetPath.EndsWith(".prefab"))
            {
                assetPath = assetPath.Substring(0, assetPath.Length - ".prefab".Length);
            }

            paths.Add(assetPath);
        }

        return paths;
    }
}
