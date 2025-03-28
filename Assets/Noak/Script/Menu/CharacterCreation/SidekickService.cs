using Synty.SidekickCharacters.API;
using Unity.FantasyKingdom;
using UnityEngine;

public static class SidekickService
{
    private static SidekickRuntime _runtime;

    public static SidekickRuntime Runtime
    {
        get
        {
            if (_runtime == null)
            {
                GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
                Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

                if (model == null || material == null)
                {
                    Debug.LogError("❌ Could not load base model or material for SidekickRuntime.");
                    return null;
                }

                _runtime = new SidekickRuntime(model, material, null, DatabaseService.Instance);
                Debug.Log("✅ SidekickRuntime initialized.");
            }

            return _runtime;
        }
    }

    public static void Reset()
    {
        _runtime = null;
    }
}
