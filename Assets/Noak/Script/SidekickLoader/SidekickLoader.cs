using UnityEngine;
using System.Collections.Generic;

public class SidekickLoader : MonoBehaviour
{
    [Header("References")]
    public Transform meshesParent;
    public Transform rootBoneTransform;
    public List<PartVariants> allParts;

    private Dictionary<string, List<GameObject>> activeParts = new(); // Modified: can be 1 or 2 active instances
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
        if (!partsCatalog.TryGetValue(partName, out var partVariants))
        {
            Debug.LogWarning($"Part {partName} not found.");
            return;
        }

        if (variantIndex < 0 || variantIndex >= partVariants.variants.Count)
        {
            Debug.LogWarning($"Variant {variantIndex} for part {partName} not found.");
            return;
        }

        // Destroy old part(s)
        if (activeParts.TryGetValue(partName, out var oldParts))
        {
            foreach (var oldPart in oldParts)
            {
                if (oldPart != null)
                    Destroy(oldPart);
            }
            activeParts.Remove(partName);
        }

        var newParts = new List<GameObject>();

        var variant = partVariants.variants[variantIndex];

        if (variant.isDualPath)
        {
            // Load Left
            if (!string.IsNullOrEmpty(variant.leftPath))
            {
                GameObject leftPrefab = Resources.Load<GameObject>(variant.leftPath);
                if (leftPrefab != null)
                {
                    GameObject instanceLeft = Instantiate(leftPrefab, meshesParent);
                    RebindBones(instanceLeft.GetComponentInChildren<SkinnedMeshRenderer>());
                    newParts.Add(instanceLeft);
                }
                else
                {
                    Debug.LogError($"Left prefab not found at path {variant.leftPath}");
                }
            }

            // Load Right
            if (!string.IsNullOrEmpty(variant.rightPath))
            {
                GameObject rightPrefab = Resources.Load<GameObject>(variant.rightPath);
                if (rightPrefab != null)
                {
                    GameObject instanceRight = Instantiate(rightPrefab, meshesParent);
                    RebindBones(instanceRight.GetComponentInChildren<SkinnedMeshRenderer>());
                    newParts.Add(instanceRight);
                }
                else
                {
                    Debug.LogError($"Right prefab not found at path {variant.rightPath}");
                }
            }
        }
        else
        {
            // Load Single
            if (!string.IsNullOrEmpty(variant.singlePath))
            {
                GameObject singlePrefab = Resources.Load<GameObject>(variant.singlePath);
                if (singlePrefab != null)
                {
                    GameObject instanceSingle = Instantiate(singlePrefab, meshesParent);
                    RebindBones(instanceSingle.GetComponentInChildren<SkinnedMeshRenderer>());
                    newParts.Add(instanceSingle);
                }
                else
                {
                    Debug.LogError($"Single prefab not found at path {variant.singlePath}");
                }
            }
        }

        if (newParts.Count > 0)
            activeParts[partName] = newParts;
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

    public void RandomizeCharacter()
    {
        foreach (var entry in partsCatalog)
        {
            int randomVariantIndex = Random.Range(0, entry.Value.variants.Count);
            EquipPart(entry.Key, randomVariantIndex);
        }
    }
}
