using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PartVariantsUpdater : EditorWindow
{
    private PartVariants partVariantsSO;
    private string baseFolderPath = "Assets/Resources/Parts/";

    [MenuItem("Tools/Vinterkyla: SideKick Editor/Update PartVariants")]
    public static void ShowWindow()
    {
        GetWindow<PartVariantsUpdater>("Update PartVariants");
    }

    private void OnGUI()
    {
        GUILayout.Label("Update Existing PartVariants", EditorStyles.boldLabel);

        partVariantsSO = (PartVariants)EditorGUILayout.ObjectField("PartVariants SO", partVariantsSO, typeof(PartVariants), false);
        baseFolderPath = EditorGUILayout.TextField("Base Parts Folder", baseFolderPath);

        if (GUILayout.Button("Update PartVariants"))
        {
            if (partVariantsSO == null)
            {
                Debug.LogError("No PartVariants selected!");
                return;
            }

            UpdatePartVariants();
        }
    }

    private void UpdatePartVariants()
    {
        if (partVariantsSO.variants == null)
            partVariantsSO.variants = new List<PartVariants.VariantEntry>();

        // Build a quick set of existing paths to avoid duplicates
        HashSet<string> existingPaths = new HashSet<string>();

        foreach (var v in partVariantsSO.variants)
        {
            if (v.isDualPath)
            {
                if (!string.IsNullOrEmpty(v.leftPath)) existingPaths.Add(v.leftPath);
                if (!string.IsNullOrEmpty(v.rightPath)) existingPaths.Add(v.rightPath);
            }
            else
            {
                if (!string.IsNullOrEmpty(v.singlePath)) existingPaths.Add(v.singlePath);
            }
        }

        if (partVariantsSO.variants.Count > 0 && partVariantsSO.variants[0].isDualPath)
        {
            Debug.Log("Updating DualPath PartVariants");

            // Load left and right parts
            var leftPaths = LoadPrefabsMatchingName(partVariantsSO.partName + "L");
            var rightPaths = LoadPrefabsMatchingName(partVariantsSO.partName + "R");

            int pairCount = Mathf.Min(leftPaths.Count, rightPaths.Count);

            for (int i = 0; i < pairCount; i++)
            {
                if (!existingPaths.Contains(leftPaths[i]) && !existingPaths.Contains(rightPaths[i]))
                {
                    var entry = new PartVariants.VariantEntry
                    {
                        isDualPath = true,
                        leftPath = leftPaths[i],
                        rightPath = rightPaths[i]
                    };
                    partVariantsSO.variants.Add(entry);
                    Debug.Log($"Added dual path: {leftPaths[i]} + {rightPaths[i]}");
                }
            }
        }
        else
        {
            Debug.Log("Updating SinglePath PartVariants");

            var singlePaths = LoadPrefabsMatchingName(partVariantsSO.partName);

            foreach (var path in singlePaths)
            {
                if (!existingPaths.Contains(path))
                {
                    var entry = new PartVariants.VariantEntry
                    {
                        isDualPath = false,
                        singlePath = path
                    };
                    partVariantsSO.variants.Add(entry);
                    Debug.Log($"Added single path: {path}");
                }
            }
        }

        EditorUtility.SetDirty(partVariantsSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Finished updating {partVariantsSO.name}!");
    }

    private List<string> LoadPrefabsMatchingName(string searchName)
    {
        List<string> matchingPaths = new List<string>();

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { baseFolderPath });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if (!assetPath.StartsWith("Assets/Resources/"))
                continue;

            if (!assetPath.EndsWith(".prefab"))
                continue;

            if (!assetPath.Contains(searchName))
                continue;

            string resourcePath = assetPath.Substring("Assets/Resources/".Length);
            resourcePath = resourcePath.Substring(0, resourcePath.Length - ".prefab".Length);

            matchingPaths.Add(resourcePath);
        }

        matchingPaths.Sort();
        return matchingPaths;
    }
}
