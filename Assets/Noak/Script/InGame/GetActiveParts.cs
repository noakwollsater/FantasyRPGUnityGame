using System.Collections.Generic;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class GetActiveParts : MonoBehaviour
    {
        public Dictionary<string, string> selectedParts = new();

        private GameObject character;

        public void GetActivePartsFromCharacter()
        {
            character = GameObject.FindGameObjectWithTag("Player");
            if (character == null)
            {
                Debug.LogWarning("⚠️ Character not found.");
                return;
            }

            var characterParts = character.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var part in characterParts)
            {
                if (part.gameObject.activeSelf)
                {
                    string partName = part.gameObject.name;
                    selectedParts[partName] = part.gameObject.activeSelf.ToString();
                }
            }
        }
    }
}
