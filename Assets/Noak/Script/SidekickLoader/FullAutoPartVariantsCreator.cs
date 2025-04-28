using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class FullAutoPartVariantsCreator : EditorWindow
{
    private string baseFolderPath = "Assets/Resources/Parts/";
    private string partName = "";
    private string partNameLeft = "";
    private string partNameRight = "";
    private bool isDualPath = false;

    [MenuItem("Tools/Full Auto PartVariants Creator")]
    public static void ShowWindow()
    {
        GetWindow<FullAutoPartVariantsCreator>("Full Auto PartVariants");
    }

    private void OnGUI()
    {
        GUILayout.Label("Full Auto PartVariants Creator", EditorStyles.boldLabel);

        baseFolderPath = EditorGUILayout.TextField("Base Parts Folder", baseFolderPath);

        isDualPath = EditorGUILayout.Toggle("Dual Path (Left/Right)", isDualPath);

        if (isDualPath)
        {
            partNameLeft = EditorGUILayout.TextField("Left Part Name", partNameLeft);
            partNameRight = EditorGUILayout.TextField("Right Part Name", partNameRight);
        }
        else
        {
            partName = EditorGUILayout.TextField("Part Name", partName);
        }

        if (GUILayout.Button("Create PartVariants"))
        {
            if (string.IsNullOrEmpty(baseFolderPath))
            {
                Debug.LogError("Base folder path is empty!");
                return;
            }

            if (isDualPath && (string.IsNullOrEmpty(partNameLeft) || string.IsNullOrEmpty(partNameRight)))
            {
                Debug.LogError("Left and/or Right part names are empty!");
                return;
            }

            if (!isDualPath && string.IsNullOrEmpty(partName))
            {
                Debug.LogError("Part name is empty!");
                return;
            }

            CreatePartVariants();
        }
    }

    private void CreatePartVariants()
    {
        // Create folder if missing
        string variantsFolder = "Assets/Resources/PartVariants";
        if (!AssetDatabase.IsValidFolder(variantsFolder))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "PartVariants");
        }

        // Create the ScriptableObject
        PartVariants newPartVariants = ScriptableObject.CreateInstance<PartVariants>();

        if (isDualPath)
        {
            newPartVariants.partName = partNameLeft.Replace("L", ""); // Optional: strip L if you want clean naming
        }
        else
        {
            newPartVariants.partName = partName;
        }

        newPartVariants.variants = new List<PartVariants.VariantEntry>();

        if (isDualPath)
        {
            // Load Left and Right parts
            var leftPaths = LoadPrefabsMatchingName(partNameLeft);
            var rightPaths = LoadPrefabsMatchingName(partNameRight);

            int pairCount = Mathf.Min(leftPaths.Count, rightPaths.Count);

            for (int i = 0; i < pairCount; i++)
            {
                var entry = new PartVariants.VariantEntry
                {
                    isDualPath = true,
                    leftPath = leftPaths[i],
                    rightPath = rightPaths[i]
                };
                newPartVariants.variants.Add(entry);
            }

            Debug.Log($"Created {pairCount} left/right pairs for {newPartVariants.partName}");
        }
        else
        {
            // Load Single parts
            var singlePaths = LoadPrefabsMatchingName(partName);

            foreach (var path in singlePaths)
            {
                var entry = new PartVariants.VariantEntry
                {
                    isDualPath = false,
                    singlePath = path
                };
                newPartVariants.variants.Add(entry);
            }

            Debug.Log($"Created {singlePaths.Count} single variants for {newPartVariants.partName}");
        }

        // Save the ScriptableObject
        string savePath = $"{variantsFolder}/{newPartVariants.partName}Variants.asset";
        AssetDatabase.CreateAsset(newPartVariants, savePath);
        EditorUtility.SetDirty(newPartVariants);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Saved new PartVariants at {savePath}");
    }

    private List<string> LoadPrefabsMatchingName(string partName)
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

            if (!assetPath.Contains(partName))
                continue;

            string resourcePath = assetPath.Substring("Assets/Resources/".Length);
            resourcePath = resourcePath.Substring(0, resourcePath.Length - ".prefab".Length);

            matchingPaths.Add(resourcePath);
        }

        matchingPaths.Sort(); // Optional: sort alphabetically

        return matchingPaths;
    }
}
