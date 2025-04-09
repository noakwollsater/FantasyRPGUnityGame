using UnityEngine;
using System;
using System.Reflection;

public static class CloneRuntimeSetup
{
    public static void Inject(GameObject clonePrefab, GameObject player)
    {
        foreach (Transform topLevel in clonePrefab.transform)
        {
            GameObject injected = new GameObject(topLevel.name);
            injected.transform.SetParent(player.transform, false);

            CopyComponents(topLevel.gameObject, injected);

            foreach (Transform child in topLevel)
            {
                CopyChildRecursive(child, injected.transform);
            }
        }
    }

    private static void CopyChildRecursive(Transform source, Transform targetParent)
    {
        GameObject copy = new GameObject(source.name);
        copy.transform.SetParent(targetParent, false);
        copy.transform.localPosition = source.localPosition;
        copy.transform.localRotation = source.localRotation;
        copy.transform.localScale = source.localScale;

        CopyComponents(source.gameObject, copy);

        foreach (Transform child in source)
        {
            CopyChildRecursive(child, copy.transform);
        }
    }

    private static void CopyComponents(GameObject from, GameObject to)
    {
        foreach (Component component in from.GetComponents<Component>())
        {
            if (component is Transform) continue;

            var type = component.GetType();
            var copy = to.AddComponent(type);

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
                {
                    try { field.SetValue(copy, field.GetValue(component)); }
                    catch { Debug.LogWarning($"⚠️ Could not copy '{field.Name}' on {type.Name}"); }
                }
            }
        }
    }
}
