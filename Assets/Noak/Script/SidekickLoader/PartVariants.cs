using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartVariants", menuName = "Sidekick/PartVariants")]
public class PartVariants : ScriptableObject
{
    public string partName;
    public List<string> prefabPaths = new();
}
