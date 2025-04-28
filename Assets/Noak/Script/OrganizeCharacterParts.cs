using System.Collections.Generic;
using UnityEngine;

public class OrganizeCharacterParts : MonoBehaviour
{
    [Tooltip("Drag the mesh folder of your character here")]
    public GameObject MeshFolder;

    [Tooltip("Drag the material you want here")]
    public Material MaterialToApply;

    private Dictionary<string, Transform> folders = new();

    private readonly List<string> categories = new()
    {
        "HEAD", "HAIR", "EBRL", "EBRR", "EYEL", "EYER", "EARL", "EARR", "FCHR", "TORS",
        "AUPL", "AUPR", "ALWL", "ALWR", "HNDL", "HNDR", "HIPS", "LEGL", "LEGR", "FOTL",
        "FOTR", "NOSE", "TETH", "TONG", "AHED", "AFAC", "ABAC", "AHPF", "AHPB", "AHPL",
        "AHPR", "ASHL", "ASHR", "AEBL", "AEBR", "AKNL", "AKNR"
    };

    private readonly List<string> categoryFolderName = new()
    {
        "HEAD", "HAIR", "EBRL", "EBRR", "EYEL", "EYER", "EARL", "EARR", "FCHR", "TORS",
        "AUPL", "AUPR", "ALWL", "ALWR", "HNDL", "HNDR", "HIPS", "LEGL", "LEGR", "FOTL",
        "FOTR", "NOSE", "TETH", "TONG", "AHED", "AFAC", "ABAC", "AHPF", "AHPB", "AHPL",
        "AHPR", "ASHL", "ASHR", "AEBL", "AEBR", "AKNL", "AKNR"
    };

    [ContextMenu("Organize Parts")]
    public void OrganizeParts()
    {
        if (MeshFolder == null)
        {
            Debug.LogError("Mesh folder not assigned!");
            return;
        }

        folders.Clear();

        // Create folders using the friendly names
        for (int i = 0; i < categories.Count; i++)
        {
            string key = categories[i].ToLower();
            string folderName = categoryFolderName[i];

            Transform folder = MeshFolder.transform.Find(folderName);
            if (folder == null)
            {
                GameObject newFolder = new GameObject(folderName);
                newFolder.transform.SetParent(MeshFolder.transform);
                folder = newFolder.transform;
            }

            folders[key] = folder;
        }

        // Move and deactivate objects with SkinnedMeshRenderer
        var skinnedMeshRenderers = MeshFolder.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var smr in skinnedMeshRenderers)
        {
            GameObject obj = smr.gameObject;
            string lowerName = obj.name.ToLower();

            for (int i = 0; i < categories.Count; i++)
            {
                string key = categories[i].ToLower();
                if (lowerName.Contains(key))
                {
                    obj.transform.SetParent(folders[key]);
                    obj.SetActive(false);
                    Debug.Log($"Moved and deactivated {obj.name} to folder {categoryFolderName[i]}");
                    break;
                }
            }
        }

        // Remove non-category top-level objects
        List<GameObject> toRemove = new();
        foreach (Transform child in MeshFolder.transform)
        {
            if (!folders.ContainsValue(child))
            {
                toRemove.Add(child.gameObject);
            }
        }

        foreach (GameObject obj in toRemove)
        {
            Debug.Log($"Removing non-category object: {obj.name}");
            DestroyImmediate(obj);
        }

        Debug.Log("Character parts organized with friendly folder names, deactivated, and cleaned up!");
    }

    [ContextMenu("Apply Material")]
    public void ApplyMaterial()
    {
        if (MaterialToApply == null)
        {
            Debug.LogError("Material not assigned!");
            return;
        }

        var skinnedMeshRenderers = MeshFolder.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var smr in skinnedMeshRenderers)
        {
            int materialCount = smr.sharedMaterials.Length;

            Material[] newMats = new Material[materialCount];
            for (int i = 0; i < materialCount; i++)
            {
                newMats[i] = MaterialToApply;
            }

            smr.materials = newMats;

            Debug.Log($"Applied {MaterialToApply.name} to all {materialCount} material slots on {smr.gameObject.name}");
        }

        Debug.Log("Material applied to all SkinnedMeshRenderers!");
    }
}