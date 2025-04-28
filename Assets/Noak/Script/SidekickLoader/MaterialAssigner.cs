using UnityEngine;
using UnityEditor;

public class MultiFolderMaterialAssigner : EditorWindow
{
    private Material materialToAssign;
    [SerializeField] private string[] targetFolders;

    [MenuItem("Tools/Assign Material To Multiple Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<MultiFolderMaterialAssigner>("Assign Material To Prefabs");
    }

    private void OnGUI()
    {
        GUILayout.Label("Assign Material to Prefabs in Multiple Folders", EditorStyles.boldLabel);

        materialToAssign = (Material)EditorGUILayout.ObjectField("Material to Assign", materialToAssign, typeof(Material), false);

        SerializedObject so = new SerializedObject(this);
        SerializedProperty foldersProp = so.FindProperty("targetFolders");

        EditorGUILayout.PropertyField(foldersProp, new GUIContent("Target Folders"), true);

        so.ApplyModifiedProperties();

        if (GUILayout.Button("Assign Material"))
        {
            if (materialToAssign == null)
            {
                Debug.LogError("Please assign a material first!");
                return;
            }

            if (targetFolders == null || targetFolders.Length == 0)
            {
                Debug.LogError("Please assign at least one folder!");
                return;
            }

            AssignMaterial();
        }
    }

    private void AssignMaterial()
    {
        int modifiedCount = 0;

        foreach (string folderPath in targetFolders)
        {
            if (string.IsNullOrEmpty(folderPath))
                continue;

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab == null)
                {
                    Debug.LogWarning($"Could not load prefab at {assetPath}");
                    continue;
                }

                bool modified = false;

                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

                foreach (Renderer renderer in renderers)
                {
                    Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        newMaterials[i] = materialToAssign;
                    }

                    renderer.sharedMaterials = newMaterials;
                    modified = true;
                }

                if (modified)
                {
                    PrefabUtility.SavePrefabAsset(prefab);
                    modifiedCount++;
                    Debug.Log($"Updated materials in prefab: {prefab.name}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Finished assigning material to {modifiedCount} prefabs!");
    }
}
