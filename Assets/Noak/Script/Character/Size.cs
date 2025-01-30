using System.Collections.Generic;
using UnityEngine;

public class Size : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] parts;
    public TriangleStatController statController; // Koppling till dina stats

    private Dictionary<SkinnedMeshRenderer, int> Heavy = new Dictionary<SkinnedMeshRenderer, int>();
    private Dictionary<SkinnedMeshRenderer, int> Buff = new Dictionary<SkinnedMeshRenderer, int>();

    void Start()
    {
        foreach (var part in parts)
        {
            for (int i = 0; i < part.sharedMesh.blendShapeCount; i++)
            {
                string blendShapeName = part.sharedMesh.GetBlendShapeName(i);
                if (blendShapeName.Contains("defaultHeavy")) // Kontrollera för "Heavy"
                {
                    Heavy.Add(part, i);
                    Debug.Log("Added " + part.name + " with blend shape: " + blendShapeName);
                }
                else if (blendShapeName.Contains("defaultBuff")) // Kontrollera för "Buff"
                {
                    Buff.Add(part, i);
                    Debug.Log("Added " + part.name + " with blend shape: " + blendShapeName);
                }
            }
        }
    }

    void Update()
    {
        // Använd stats för att sätta blend shapes
        UpdateBlendShapes();
    }

    private void UpdateBlendShapes()
    {
        // Hämta stats
        float muscle = statController.muscle;
        float fat = statController.fat;

        // Sätt blend shapes baserat på stats
        foreach (var part in Heavy.Keys)
        {
            part.SetBlendShapeWeight(Heavy[part], fat); // Fat påverkar "Heavy"
        }

        foreach (var part in Buff.Keys)
        {
            part.SetBlendShapeWeight(Buff[part], muscle); // Muscle påverkar "Buff"
        }
    }
}
