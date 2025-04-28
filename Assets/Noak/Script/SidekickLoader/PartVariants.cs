using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartVariants", menuName = "Sidekick/PartVariants")]
public class PartVariants : ScriptableObject
{
    public string partName;

    [System.Serializable]
    public class VariantEntry
    {
        public bool isDualPath; // true = left + right, false = only singlePath

        public string singlePath; // Used if isDualPath == false

        public string leftPath;   // Used if isDualPath == true
        public string rightPath;  // Used if isDualPath == true
    }

    public List<VariantEntry> variants = new();
}
