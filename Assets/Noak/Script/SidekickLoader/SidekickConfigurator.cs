using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mightland.Scripts.SK
{
    public class SidekickConfigurator : MonoBehaviour
    {
        [Header("Blend Shapes")]
        [Range(-100, 100)] public float bodyTypeBlendValue;

        [Range(-100, 100)] public float bodySizeValue;
        [Range(-100, 100)] public float musclesBlendValue;

        [Header("Components")]
        public Transform meshesTransform;

        public Transform rootBoneTransform;

        // Internal
        public List<SidekickMesh> meshParts = new();
        private readonly Dictionary<string, Transform> _boneMap = new();
        private readonly Dictionary<string, SidekickMesh> _meshPartMap = new();

        [HideInInspector]
        public List<string> partGroups = new();

        [HideInInspector]
        public List<MeshPartList> meshPartsList = new();

        [HideInInspector]
        public List<int> meshPartsActive = new();

#if UNITY_EDITOR
        [Header("Build Options")]
        public Material defaultMaterial;

        public List<string> assetsPath = new();
#endif

        private void OnEnable()
        {
            UpdateBoneMap(_boneMap, rootBoneTransform, "root");

            _meshPartMap.Clear();
            foreach (var meshPart in meshParts)
            {
                _meshPartMap[meshPart.partName] = meshPart;
            }
        }

        public void ApplyBlendShapes()
        {
            for (var i = 0; i < partGroups.Count; i++)
            {
                var activeMesh = meshPartsList[i].items[meshPartsActive[i]];
                if (activeMesh != null)
                {
                    OnMeshChange(i, meshPartsActive[i]);
                }
            }
        }

        private void OnMeshChange(int index, int meshIndex)
        {
            if (meshIndex == 0) return;

            var meshPart = meshPartsList[index].items[meshIndex];
            var partGroup = meshPart.partGroup;

            // Revert dynamic bone positions and rotations to their original values
            for (var i = 0; i < meshPart.dynamicBones.Count; i++)
            {
                var bone = meshPart.dynamicBones[i];
                bone.localPosition = meshPart.originalDynamicBones[i].localPosition;
                bone.localRotation = meshPart.originalDynamicBones[i].localRotation;
                bone.localScale = meshPart.originalDynamicBones[i].localScale;
            }

            // Apply blend shape values
            ApplyBlendShapeValues(meshPart.skinnedMeshRenderer);

            // Apply blend offsets to attachments
            float bodySizeHeavyBlendValue;
            float bodySizeSkinnyBlendValue;

            switch (bodySizeValue)
            {
                case > 0:
                    bodySizeHeavyBlendValue = bodySizeValue;
                    bodySizeSkinnyBlendValue = 0;
                    break;
                case < 0:
                    bodySizeHeavyBlendValue = 0;
                    bodySizeSkinnyBlendValue = -bodySizeValue;
                    break;
                default:
                    bodySizeHeavyBlendValue = 0;
                    bodySizeSkinnyBlendValue = 0;
                    break;
            }

            var attachmentNames = new List<string>
            {
                "AHED", "AFAC", "ABAC", "AHPF", "AHPB", "AHPL", "AHPR", "ASHL", "ASHR", "AEBL", "AEBR", "AKNL", "AKNR"
            };
            if (attachmentNames.Contains(partGroup))
            {
                foreach (var bone in meshPart.dynamicBones)
                {
                    if (bone.name == PartToBoneName(partGroup))
                    {
                        ApplyAttachmentOffset(partGroup, (bodyTypeBlendValue + 100) / 2, bodySizeSkinnyBlendValue, bodySizeHeavyBlendValue, (musclesBlendValue + 100) / 2, bone);
                        break;
                    }
                }
            }
        }

        private void ApplyAttachmentOffset(string part, float fem, float skinny, float heavy, float muscle, Transform bone)
        {
            fem /= 100;
            skinny /= 100;
            heavy /= 100;
            muscle /= 100;

            var offsetPosition = Vector3.zero;
            var offsetRotation = Quaternion.identity;

            if (part == "ABAC") // backAttach
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.009999999776482582f, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.07999999821186066f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.009999999776482582f, 0), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.03999999910593033f, 0.03999999910593033f, 0), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(356, 0, 0)), muscle);
            }
            else if (part == "AHPF") // hipAttachFront
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.004999999888241291f, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.05999999865889549f, 0.15000000596046448f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.007000000216066837f, 0), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.013000000268220901f, 0), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(15.000001907348633f, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(2, 0, 0)), muscle);
            }
            else if (part == "AHPB") // hipAttachBack
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.014999999664723873f, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.004999999888241291f, -0.14000000059604645f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.004999999888241291f, 0), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.009999999776482582f, -0.019999999552965164f, 0), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(3, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(352, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "AHPL") // hipAttach_l
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0.004999999888241291f), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0.12999999523162842f), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -0.009999999776482582f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0.014999999664723873f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "AHPR") // hipAttach_r
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -0.004999999888241291f), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -0.12999999523162842f), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0.00800000037997961f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, -0.009999999776482582f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "ASHL") // shoulderAttach_l
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.009999999776482582f, 0, -0.014999999664723873f), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.03999999910593033f, 0.009999999776482582f, 0.02800000086426735f), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.014999999664723873f, 0, -0.009999999776482582f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.029999999329447746f, 0, 0.05000000074505806f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "ASHR") // shoulderAttach_r
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.009999999776482582f, 0, -0.014999999664723873f), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.03999999910593033f, -0.009999999776482582f, 0.02800000086426735f), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0.014999999664723873f, 0, -0.009999999776482582f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(-0.029999999329447746f, 0, 0.05000000074505806f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "AEBL") // elbowAttach_l
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.009999999776482582f, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.02500000037252903f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.014999999664723873f, 0.009999999776482582f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.05999999865889549f, 0.019999999552965164f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 2, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(13.000000953674316f, 0, 0)), muscle);
            }
            else if (part == "AEBR") // elbowAttach_r
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.009999999776482582f, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.03500000014901161f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.014999999664723873f, 0.009999999776482582f), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.05999999865889549f, 0.019999999552965164f), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 358, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(13.000000953674316f, 0, 0)), muscle);
            }
            else if (part == "AKNL") // kneeAttach_l
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.014999999664723873f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.009999999776482582f, 0), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.0020000000949949026f, 0), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 20.000001907348633f, 0)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }
            else if (part == "AKNR") // kneeAttach_r
            {
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0, 0), fem);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.014999999664723873f, 0), heavy);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, -0.009999999776482582f, 0), skinny);
                offsetPosition += Vector3.Lerp(Vector3.zero, new Vector3(0, 0.0020000000949949026f, 0), muscle);

                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), fem);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 15.000000953674316f)), heavy);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), skinny);
                offsetRotation *= Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(new Vector3(0, 0, 0)), muscle);
            }

            bone.localPosition += offsetPosition;
            bone.localRotation *= offsetRotation;
        }

        private void ApplyBlendShapeValues(SkinnedMeshRenderer skinnedMesh)
        {
            float bodySizeHeavyBlendValue;
            float bodySizeSkinnyBlendValue;

            switch (bodySizeValue)
            {
                case > 0:
                    bodySizeHeavyBlendValue = bodySizeValue;
                    bodySizeSkinnyBlendValue = 0;
                    break;
                case < 0:
                    bodySizeHeavyBlendValue = 0;
                    bodySizeSkinnyBlendValue = -bodySizeValue;
                    break;
                default:
                    bodySizeHeavyBlendValue = 0;
                    bodySizeSkinnyBlendValue = 0;
                    break;
            }

            var sharedMesh = skinnedMesh.sharedMesh;
            for (var i = 0; i < sharedMesh.blendShapeCount; i++)
            {
                var blendName = sharedMesh.GetBlendShapeName(i);
                if (blendName.Contains("masculineFeminine"))
                {
                    skinnedMesh.SetBlendShapeWeight(i, (bodyTypeBlendValue + 100) / 2);
                }
                else if (blendName.Contains("defaultSkinny"))
                {
                    skinnedMesh.SetBlendShapeWeight(i, bodySizeSkinnyBlendValue);
                }
                else if (blendName.Contains("defaultHeavy"))
                {
                    skinnedMesh.SetBlendShapeWeight(i, bodySizeHeavyBlendValue);
                }
                else if (blendName.Contains("defaultBuff"))
                {
                    skinnedMesh.SetBlendShapeWeight(i, (musclesBlendValue + 100) / 2);
                }
            }
        }

#if UNITY_EDITOR
        public void PopulateMeshParts()
        {
            CleanMeshes();

            UpdateBoneMap(_boneMap, rootBoneTransform, "root");

            ImportMeshes();

            BuildActiveMeshes();

            AssignDefaultMaterial();
        }

        private void AssignDefaultMaterial()
        {
            foreach (var meshPart in meshParts)
            {
                var mesh = meshPart.skinnedMeshRenderer;

                var sharedMaterials = mesh.sharedMaterials;

                for (var materialIndex = 0; materialIndex < sharedMaterials.Length; materialIndex++)
                {
                    sharedMaterials[materialIndex] = defaultMaterial;
                }

                mesh.sharedMaterials = sharedMaterials;
            }
        }

        private void BuildActiveMeshes()
        {
            partGroups.Clear();
            meshPartsList.Clear();
            meshPartsActive.Clear();

            // Add all mesh parts to the active meshes
            foreach (var meshPart in meshParts)
            {
                var meshName = meshPart.partName.Split('_')[^2][2..];

                var index = partGroups.IndexOf(meshName);
                if (index != -1)
                {
                    meshPartsList[index].items.Add(meshPart);
                }
                else
                {
                    partGroups.Add(meshName);
                    meshPartsList.Add(new MeshPartList
                    {
                        items = new List<SidekickMesh> { null, meshPart }
                    });
                    meshPartsActive.Add(0);
                }
            }

            for (var i = 0; i < partGroups.Count; i++)
            {
                for (var j = 0; j < meshPartsList[i].items.Count; j++)
                {
                    var meshPart = meshPartsList[i].items[j];

                    if (i <= 23 && j == 1)
                    {
                        meshPart.meshTransform.gameObject.SetActive(true);
                        meshPartsActive[i] = j;
                    }
                    else if (j != 0)
                    {
                        meshPart.meshTransform.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void ImportMeshes()
        {
            // Reset the root bone transform
            rootBoneTransform.transform.localPosition = Vector3.zero;
            rootBoneTransform.transform.localRotation = Quaternion.identity;
            rootBoneTransform.transform.localScale = Vector3.one;

            // Reset the meshes transform
            meshesTransform.transform.localPosition = Vector3.zero;
            meshesTransform.transform.localRotation = Quaternion.identity;
            meshesTransform.transform.localScale = Vector3.one;

            // Clear the mesh parts list
            meshParts.Clear();

            // Scan all assets in the assetsPath list and instantiate them
            foreach (var assetPath in assetsPath)
            {
                var guids = AssetDatabase.FindAssets("t:GameObject", new[] { assetPath });

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                    if (obj == null) continue;

                    // Instantiate the object inside the temporary GameObject
                    var instance = Instantiate(obj, meshesTransform);
                    instance.name = instance.name.Replace("(Clone)", "");

                    var skinnedMeshRenderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();

                    // var partType = instance.name.Split('_')[^2][2..];
                    // var isAttachment = partType is "AHED" or "AFAC" or "ABAC" or "AHPF" or "AHPB" or "AHPL" or "AHPR" or "ASHL" or "ASHR" or "AEBL" or "AEBR" or "AKNL" or "AKNR";

                    // Get a copy of skinnedMeshRenderer.bones
                    var originalBones = new Transform[skinnedMeshRenderer.bones.Length];
                    for (var i = 0; i < skinnedMeshRenderer.bones.Length; i++)
                    {
                        originalBones[i] = skinnedMeshRenderer.bones[i];
                    }

                    var rootBone = instance.transform.Find("root");

                    // Add the mesh part to the list
                    var sidekickMesh = new SidekickMesh
                    {
                        partName = instance.name,
                        partGroup = instance.name.Split('_')[^2][2..],
                        meshTransform = instance.transform,
                        skinnedMeshRenderer = skinnedMeshRenderer,
                        rootBoneTransform = rootBone,
                        dynamicBones = new List<Transform>(),
                        originalDynamicBones = new List<Transform>(),
                    };

                    // Add the mesh part to the list and dictionary
                    meshParts.Add(sidekickMesh);

                    // Bind the bones
                    CreateOrModifyBone(rootBoneTransform.parent, sidekickMesh.rootBoneTransform, "root");
                    BindNewBones(sidekickMesh, sidekickMesh.rootBoneTransform, originalBones);
                }
            }

            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Clean Meshes")]
        private void CleanMeshes()
        {
            // Create root bone
            rootBoneTransform = transform.Find("root");
            if (!rootBoneTransform)
            {
                var go = new GameObject("root");
                rootBoneTransform = go.transform;
                rootBoneTransform.transform.SetParent(transform);
                rootBoneTransform.transform.localPosition = Vector3.zero;
                rootBoneTransform.transform.localRotation = Quaternion.identity;
                rootBoneTransform.transform.localScale = Vector3.one;
            }
            else
            {
                rootBoneTransform.transform.localPosition = Vector3.zero;
                rootBoneTransform.transform.localRotation = Quaternion.identity;
                rootBoneTransform.transform.localScale = Vector3.one;
                // Delete all children
                while (rootBoneTransform.childCount != 0)
                {
                    DestroyImmediate(rootBoneTransform.GetChild(0).gameObject);
                }
            }

            // Create a transform names meshes, or clear it if it already exists
            meshesTransform = transform.Find("meshes");
            if (!meshesTransform)
            {
                var go = new GameObject("meshes");
                meshesTransform = go.transform;
                meshesTransform.transform.SetParent(transform);
                meshesTransform.transform.localPosition = Vector3.zero;
                meshesTransform.transform.localRotation = Quaternion.identity;
                meshesTransform.transform.localScale = Vector3.one;
            }
            else
            {
                meshesTransform.transform.localPosition = Vector3.zero;
                meshesTransform.transform.localRotation = Quaternion.identity;
                meshesTransform.transform.localScale = Vector3.one;
                while (meshesTransform.childCount != 0)
                {
                    DestroyImmediate(meshesTransform.GetChild(0).gameObject);
                }
            }

            // Clear the mesh parts list
            meshParts.Clear();
            partGroups.Clear();
            meshPartsList.Clear();
        }

#endif

        private void UpdateBoneMap(Dictionary<string, Transform> map, Transform root, string path)
        {
            if (path == "root")
            {
                map.Clear();
            }

            map[path] = root;

            foreach (Transform child in root)
            {
                UpdateBoneMap(map, child, path + "/" + child.name);
            }
        }

        private void CreateOrModifyBone(Transform parentTransform, Transform newBone, string path)
        {
            // Check the children of the parent transform to see if the bone already exists
            var existingBone = parentTransform.Find(newBone.name);
            if (existingBone)
            {
                if (existingBone.parent != parentTransform)
                {
                    Debug.LogWarning($"Bone {newBone.name} has a different parent than expected.");
                }

                existingBone.localPosition = newBone.localPosition;
                existingBone.localRotation = newBone.localRotation;
                existingBone.localScale = newBone.localScale;

                _boneMap[path] = existingBone;

                foreach (Transform bone in newBone)
                {
                    CreateOrModifyBone(existingBone, bone, path + "/" + bone.name);
                }
            }
            else
            {
                var newBoneInstance = new GameObject(newBone.name).transform;
                newBoneInstance.SetParent(parentTransform);
                newBoneInstance.localPosition = newBone.localPosition;
                newBoneInstance.localRotation = newBone.localRotation;
                newBoneInstance.localScale = newBone.localScale;

                _boneMap[path] = newBoneInstance;

                foreach (Transform bone in newBone)
                {
                    CreateOrModifyBone(newBoneInstance, bone, path + "/" + bone.name);
                }
            }
        }

        private void BindNewBones(SidekickMesh sidekickMesh, Transform root, Transform[] originalBones)
        {
            var boneMap = new Dictionary<string, Transform>();
            UpdateBoneMap(boneMap, root, "root");

            var reverseBoneMap = new Dictionary<Transform, string>();
            foreach (var (key, value) in boneMap)
            {
                reverseBoneMap[value] = key;
            }

            var dynamicBones = new List<Transform>();
            var originalDynamicBones = new List<Transform>();

            var newBones = new Transform[originalBones.Length];
            for (var i = 0; i < originalBones.Length; i++)
            {
                var boneName = reverseBoneMap[originalBones[i]];
                newBones[i] = _boneMap[boneName];

                if (boneName.Contains("dyn") || PartToBoneName(sidekickMesh.partGroup) == newBones[i].name)
                {
                    dynamicBones.Add(newBones[i]);
                    originalDynamicBones.Add(originalBones[i]);
                }
            }

            sidekickMesh.skinnedMeshRenderer.bones = newBones;
            sidekickMesh.skinnedMeshRenderer.rootBone = _boneMap["root"];

            sidekickMesh.dynamicBones = dynamicBones;
            sidekickMesh.originalDynamicBones = originalDynamicBones;
        }

        private string PartToBoneName(string partGroup)
        {
            return partGroup switch
            {
                "AHED" => "headAttach",
                "AFAC" => "faceAttach",
                "ABAC" => "backAttach",
                "AHPF" => "hipAttachFront",
                "AHPB" => "hipAttachBack",
                "AHPL" => "hipAttach_l",
                "AHPR" => "hipAttach_r",
                "ASHL" => "shoulderAttach_l",
                "ASHR" => "shoulderAttach_r",
                "AEBL" => "elbowAttach_l",
                "AEBR" => "elbowAttach_r",
                "AKNL" => "kneeAttach_l",
                "AKNR" => "kneeAttach_r",
                _ => ""
            };
        }
    }

    [Serializable]
    public class SidekickMesh
    {
        public string partName;
        public string partGroup;
        public Transform meshTransform;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public Transform rootBoneTransform;
        public List<Transform> dynamicBones;
        public List<Transform> originalDynamicBones;
    }

    [Serializable]
    public class MeshPartList
    {
        public List<SidekickMesh> items = new();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SidekickConfigurator))]
    public class SidekickConfiguratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var configurator = (SidekickConfigurator)target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            // Info box
            if (GUILayout.Button("Populate Mesh Parts"))
            {
                configurator.PopulateMeshParts();
            }

            EditorGUILayout.HelpBox("Populates the mesh parts from the assets in the assetsPath list.", MessageType.Info);

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply Blend Shapes"))
            {
                configurator.ApplyBlendShapes();
            }

            EditorGUILayout.HelpBox("Applies the current blend shape values to the meshes.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Part Selection", EditorStyles.boldLabel);

            for (var i = 0; i < configurator.partGroups.Count; i++)
            {
                var partGroup = configurator.partGroups[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(partGroup, EditorStyles.boldLabel, GUILayout.Width(100));

                var activeIndex = configurator.meshPartsActive[i];
                EditorGUILayout.LabelField(activeIndex.ToString(), GUILayout.Width(20));

                if (GUILayout.Button("<", GUILayout.Width(25)))
                {
                    configurator.meshPartsActive[i] = activeIndex == 0 ? configurator.meshPartsList[i].items.Count - 1 : activeIndex - 1;

                    if (configurator.meshPartsList[i].items[activeIndex].meshTransform)
                    {
                        configurator.meshPartsList[i].items[activeIndex].meshTransform.gameObject.SetActive(false);
                    }

                    if (configurator.meshPartsList[i].items[configurator.meshPartsActive[i]].meshTransform)
                    {
                        configurator.meshPartsList[i].items[configurator.meshPartsActive[i]].meshTransform.gameObject.SetActive(true);
                    }

                    configurator.ApplyBlendShapes();
                }

                if (GUILayout.Button(">", GUILayout.Width(25)))
                {
                    configurator.meshPartsActive[i] = activeIndex == configurator.meshPartsList[i].items.Count - 1 ? 0 : activeIndex + 1;

                    if (configurator.meshPartsList[i].items[activeIndex].meshTransform)
                    {
                        configurator.meshPartsList[i].items[activeIndex].meshTransform.gameObject.SetActive(false);
                    }

                    if (configurator.meshPartsList[i].items[configurator.meshPartsActive[i]].meshTransform)
                    {
                        configurator.meshPartsList[i].items[configurator.meshPartsActive[i]].meshTransform.gameObject.SetActive(true);
                    }

                    configurator.ApplyBlendShapes();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Cycle All Parts Forward"))
            {
                for (int i = 0; i < configurator.partGroups.Count; i++)
                {
                    int activeIndex = configurator.meshPartsActive[i];
                    int nextIndex = activeIndex == configurator.meshPartsList[i].items.Count - 1 ? 0 : activeIndex + 1;

                    if (configurator.meshPartsList[i].items[activeIndex]?.meshTransform)
                    {
                        configurator.meshPartsList[i].items[activeIndex].meshTransform.gameObject.SetActive(false);
                    }

                    if (configurator.meshPartsList[i].items[nextIndex]?.meshTransform)
                    {
                        configurator.meshPartsList[i].items[nextIndex].meshTransform.gameObject.SetActive(true);
                    }

                    configurator.meshPartsActive[i] = nextIndex;
                }

                configurator.ApplyBlendShapes();
            }

        }
    }
#endif
}