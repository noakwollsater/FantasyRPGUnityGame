using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class SidekickPrefabGenerator : EditorWindow
{
    private static string sourceRoot = "Assets/Synty/SidekickCharacters/Resources/Meshes";
    private static string outputFolder = "Assets/Resources/CharacterParts";

    [MenuItem("Tools/Sidekick/Generate Character Part Prefabs")]
    public static void GeneratePrefabs()
    {
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        string[] fbxPaths = Directory.GetFiles(sourceRoot, "*.fbx", SearchOption.AllDirectories);

        int createdCount = 0;

        foreach (string fbxPath in fbxPaths)
        {
            GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbxAsset == null) continue;

            // Find first child with SkinnedMeshRenderer
            GameObject preview = PrefabUtility.InstantiatePrefab(fbxAsset) as GameObject;
            SkinnedMeshRenderer smr = preview.GetComponentInChildren<SkinnedMeshRenderer>();

            if (smr == null)
            {
                Debug.LogWarning($"No SkinnedMeshRenderer found in {fbxPath}");
                GameObject.DestroyImmediate(preview);
                continue;
            }

            GameObject partObject = smr.gameObject;

            // Create clean prefab
            string prefabName = fbxAsset.name + ".prefab";
            string outputPath = Path.Combine(outputFolder, prefabName).Replace("\\", "/");

            // Detach from any parent so it doesn't carry FBX hierarchy
            GameObject cleanPart = GameObject.Instantiate(partObject);
            cleanPart.name = fbxAsset.name;

            PrefabUtility.SaveAsPrefabAsset(cleanPart, outputPath);
            GameObject.DestroyImmediate(cleanPart);
            GameObject.DestroyImmediate(preview);

            createdCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Generated {createdCount} character part prefabs into: {outputFolder}");
    }
}
