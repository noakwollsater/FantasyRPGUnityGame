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
            var material = Addressables.LoadAssetAsync<Material>("M_BaseMaterial").WaitForCompletion();
            RuntimeInstance = new SidekickRuntime(model, material, null, new DatabaseManager());
        }
    }
}