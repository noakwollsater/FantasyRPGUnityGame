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
                if (blendShapeName.Contains("defaultHeavy")) // Kontrollera f�r "Heavy"
                {
                    Heavy.Add(part, i);
                    Debug.Log("Added " + part.name + " with blend shape: " + blendShapeName);
                }
                else if (blendShapeName.Contains("defaultBuff")) // Kontrollera f�r "Buff"
                {
                    Buff.Add(part, i);
                    Debug.Log("Added " + part.name + " with blend shape: " + blendShapeName);
                }
            }
        }
    }

    void Update()
    {
        // Anv�nd stats f�r att s�tta blend shapes
        UpdateBlendShapes();
    }

    private void UpdateBlendShapes()
    {
        // H�mta stats
        float muscle = statController.muscle;
        float fat = statController.fat;

        // S�tt blend shapes baserat p� stats
        foreach (var part in Heavy.Keys)
        {
            part.SetBlendShapeWeight(Heavy[part], fat); // Fat p�verkar "Heavy"
        }

        foreach (var part in Buff.Keys)
        {
            part.SetBlendShapeWeight(Buff[part], muscle); // Muscle p�verkar "Buff"
        }
    }
}
