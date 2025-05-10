using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using UnityEngine.AddressableAssets;
using UnityEngine;

public static class CharacterRuntimeManager
{
    public static SidekickRuntime RuntimeInstance;
    public static void InitIfNeeded()
    {
        if (RuntimeInstance == null)
        {
            var model = Addressables.LoadAssetAsync<GameObject>("SK_BaseModel").WaitForCompletion();
            Material material = Resources.Load<Material>("Materials/M_BaseMaterial");
            RuntimeInstance = new SidekickRuntime(model, material, null, new DatabaseManager());
        }
    }
}