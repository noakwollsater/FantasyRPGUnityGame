using System.Collections.Generic;
using UnityEngine;

public class CharacterPartAttacher : MonoBehaviour
{
    [Header("Reference to character rig root (ex: 'root')")]
    public Transform playerRootBone;

    [Header("Optional parent for the part (ex: TorsoContainer)")]
    public Transform partParent;

    /// <summary>
    /// Call this to attach a new part prefab (e.g. torso) to the character.
    /// </summary>
    /// <param name="partPrefab">The part prefab to attach</param>
    public void AttachPart(GameObject partPrefab)
    {
        if (partPrefab == null || playerRootBone == null)
        {
            Debug.LogWarning("Missing prefab or root bone.");
            return;
        }

        // Instantiate part
        GameObject partInstance = Instantiate(partPrefab, partParent != null ? partParent : transform);
        partInstance.name = partPrefab.name;

        SkinnedMeshRenderer smr = partInstance.GetComponentInChildren<SkinnedMeshRenderer>();
        if (smr == null)
        {
            Debug.LogWarning("No SkinnedMeshRenderer found on part.");
            return;
        }

        // Rebind bones to player's rig
        RebindBones(smr, playerRootBone);
    }

    /// <summary>
    /// Rebind bones in a SkinnedMeshRenderer to match the player's rig.
    /// </summary>
    private void RebindBones(SkinnedMeshRenderer partRenderer, Transform playerRoot)
    {
        Dictionary<string, Transform> boneMap = CreateBoneMap(playerRoot);
        Transform[] newBones = new Transform[partRenderer.bones.Length];

        for (int i = 0; i < partRenderer.bones.Length; i++)
        {
            string boneName = partRenderer.bones[i].name;

            if (boneMap.TryGetValue(boneName, out Transform newBone))
            {
                newBones[i] = newBone;
            }
            else
            {
                Debug.LogWarning($"Bone not found in player rig: {boneName}");
            }
        }

        partRenderer.bones = newBones;

        if (boneMap.TryGetValue(partRenderer.rootBone.name, out Transform newRoot))
        {
            partRenderer.rootBone = newRoot;
        }
        else if (boneMap.ContainsKey("root"))
        {
            partRenderer.rootBone = boneMap["root"];
        }
        else
        {
            Debug.LogWarning("No matching root bone found for part.");
        }
    }

    /// <summary>
    /// Builds a map of bone name -> Transform from the player's rig.
    /// </summary>
    private Dictionary<string, Transform> CreateBoneMap(Transform root)
    {
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in root.GetComponentsInChildren<Transform>())
        {
            if (!boneMap.ContainsKey(bone.name))
                boneMap.Add(bone.name, bone);
        }
        return boneMap;
    }
}
