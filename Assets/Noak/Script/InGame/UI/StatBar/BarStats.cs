using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class BarStats : MonoBehaviour
    {
        [SerializeField] LoadCharacterData _loadCharacterData;
        void Start()
        {
            if (_loadCharacterData == null)
            {
                Debug.LogError("LoadCharacterData is not assigned in BarStats.");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
