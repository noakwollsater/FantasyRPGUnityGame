using System.Collections.Generic;
using UnityEngine;

public class OrganizeCharacterParts : MonoBehaviour
{
    [Tooltip("Drag the mesh folder of your character here")]
    public GameObject MeshFolder;

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
        "Head", "Hair", "Eyebrow_Left", "Eyebrow_Right", "Eye_Left", "Eye_Right", "Ear_Left", "Ear_Right",
        "FacialHair", "Torso", "Upper_Arm_Left", "Upper_Arm_Right", "Lower_Arm_Left", "Lower_Arm_Right", "Hand_Left", "Hand_Right",
        "Hips", "Leg_Left", "Leg_Right", "Foot_Left", "Foot_Right", "Nose", "Teeth", "Tongue", "Attachment_Head", "Attachment_Face",
        "Attachment_Back", "Attachment_Hip_Front", "Attachment_Hip_Back", "Attachment_Hip_Left", "Attachment_Hip_Right",
        "Attachment_Shoulder_Left", "Attachment_Shoulder_Right", "Attachment_Elbow_Left", "Attachment_Elbow_Right",
        "Attachment_Knee_Left", "Attachment_Knee_Right"
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
}