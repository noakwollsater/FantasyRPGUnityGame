using UnityEngine;
using System.Collections.Generic;

public class SidekickLoader : MonoBehaviour
{
    [Header("References")]
    public Transform meshesParent; 
    public Transform rootBoneTransform;  
    public List<PartVariants> allParts; 

    private Dictionary<string, GameObject> activeParts = new();
    public Dictionary<string, PartVariants> partsCatalog = new();
    private Dictionary<string, Transform> boneMap = new();

    private void Awake()
    {
        BuildBoneMap();
        SetupCatalog();
        LoadDefaultParts();
    }

    private void BuildBoneMap()
    {
        boneMap.Clear();
        foreach (var bone in rootBoneTransform.GetComponentsInChildren<Transform>(true))
        {
            boneMap[bone.name] = bone;
        }
    }

    private void SetupCatalog()
    {
        foreach (var part in allParts)
        {
            if (part != null && !string.IsNullOrEmpty(part.partName))
                partsCatalog[part.partName] = part;
        }
    }

    private void LoadDefaultParts()
    {
        foreach (var entry in partsCatalog)
        {
            EquipPart(entry.Key, 0);
        }
    }

    public void EquipPart(string partName, int variantIndex)
    {
        if (!partsCatalog.TryGetValue(partName, out var variants))
        {
            Debug.LogWarning($"Part {partName} not found.");
            return;
        }

        if (variantIndex < 0 || variantIndex >= variants.prefabPaths.Count)
        {
            Debug.LogWarning($"Variant {variantIndex} for part {partName} not found.");
            return;
        }

        if (activeParts.TryGetValue(partName, out var oldPart))
        {
            Destroy(oldPart);
        }

        string path = variants.prefabPaths[variantIndex];
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path {path}");
            return;
        }

        GameObject instance = Instantiate(prefab, meshesParent);
        RebindBones(instance.GetComponentInChildren<SkinnedMeshRenderer>());
        activeParts[partName] = instance;
    }

    private void RebindBones(SkinnedMeshRenderer smr)
    {
        if (smr == null)
            return;

        var newBones = new Transform[smr.bones.Length];
        for (int i = 0; i < smr.bones.Length; i++)
        {
            string boneName = smr.bones[i].name;
            if (boneMap.TryGetValue(boneName, out var mappedBone))
            {
                newBones[i] = mappedBone;
            }
            else
            {
                Debug.LogWarning($"Bone {boneName} not found in rig.");
            }
        }

        smr.bones = newBones;
        smr.rootBone = rootBoneTransform;
    }
}
