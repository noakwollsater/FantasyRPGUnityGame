using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.FantasyKingdom.EditorTools
{
    public class OrganizeMeshes : EditorWindow
    {
        private string basePath = "Assets/Resources/Parts";
        [SerializeField] private string[] targetPaths;

        private readonly List<string> categories = new()
        {
            "HEAD", "HAIR", "EBRL", "EBRR", "EYEL", "EYER", "EARL", "EARR", "FCHR", "TORS",
            "AUPL", "AUPR", "ALWL", "ALWR", "HNDL", "HNDR", "HIPS", "LEGL", "LEGR", "FOTL",
            "FOTR", "NOSE", "TETH", "TONG", "AHED", "AFAC", "ABAC", "AHPF", "AHPB", "AHPL",
            "AHPR", "ASHL", "ASHR", "AEBL", "AEBR", "AKNL", "AKNR", "WRAP"
        };

        [MenuItem("Tools/Vinterkyla: SideKick Editor/Organize Meshes")]
        public static void ShowWindow()
        {
            GetWindow<OrganizeMeshes>("Organize Meshes");
        }

        private void OnGUI()
        {
            GUILayout.Label("Organize Meshes into Prefabs", EditorStyles.boldLabel);

            SerializedObject so = new SerializedObject(this);
            SerializedProperty pathsProperty = so.FindProperty("targetPaths");

            EditorGUILayout.PropertyField(pathsProperty, new GUIContent("Target Paths"), true);

            so.ApplyModifiedProperties();

            basePath = EditorGUILayout.TextField("Base Path", basePath);

            if (GUILayout.Button("Organize Parts"))
            {
                OrganizeParts();
            }
        }

        private void OrganizeParts()
        {
            if (targetPaths == null || targetPaths.Length == 0)
            {
                Debug.LogError("No target paths assigned!");
                return;
            }

            foreach (string targetPath in targetPaths)
            {
                string[] guids = AssetDatabase.FindAssets("t:GameObject", new[] { targetPath });

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject fbxRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (fbxRoot == null)
                    {
                        Debug.LogWarning($"Could not load GameObject at {assetPath}");
                        continue;
                    }

                    // Leta rätt på första barnet som har SkinnedMeshRenderer
                    SkinnedMeshRenderer smr = null;
                    Transform modelNode = null;

                    foreach (Transform child in fbxRoot.transform)
                    {
                        var candidate = child.GetComponent<SkinnedMeshRenderer>();
                        if (candidate != null)
                        {
                            smr = candidate;
                            modelNode = child;
                            break;
                        }
                    }

                    if (smr == null || modelNode == null)
                    {
                        Debug.LogWarning($"No SkinnedMeshRenderer found in FBX root {fbxRoot.name}, skipping.");
                        continue;
                    }

                    string matchedCategory = null;
                    string upperName = smr.name.ToUpper();

                    foreach (string cat in categories)
                    {
                        if (upperName.Contains(cat))
                        {
                            matchedCategory = cat;
                            break;
                        }
                    }

                    if (matchedCategory != null)
                    {
                        string categoryFolder = Path.Combine(basePath, matchedCategory).Replace("\\", "/");
                        if (!AssetDatabase.IsValidFolder(categoryFolder))
                        {
                            string parentFolder = Path.GetDirectoryName(categoryFolder).Replace("\\", "/");
                            string newFolderName = Path.GetFileName(categoryFolder);
                            AssetDatabase.CreateFolder(parentFolder, newFolderName);
                        }

                        string prefabPath = Path.Combine(categoryFolder, smr.name + ".prefab").Replace("\\", "/");

                        if (File.Exists(prefabPath))
                        {
                            Debug.Log($"Prefab already exists, skipping: {prefabPath}");
                            continue;
                        }

                        GameObject newPart = Instantiate(modelNode.gameObject);
                        newPart.name = smr.name;

                        PrefabUtility.SaveAsPrefabAsset(newPart, prefabPath);
                        DestroyImmediate(newPart);

                        Debug.Log($"Saved prefab: {prefabPath}");
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown category for: {smr.name}");
                    }

                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Organizing completed!");
        }
    }
}