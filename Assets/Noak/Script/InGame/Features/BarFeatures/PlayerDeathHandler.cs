using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Traits;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class PlayerDeathHandler : MonoBehaviour
    {
        [SerializeField] private LoadCharacter _loadCharacter;
        [SerializeField] private LoadCharacterData _loadCharacterData;
        [SerializeField] private HotbarStats _hotbarStats;
        [SerializeField] private LevelManager _levelManager;

        [SerializeField] private GameObject deathscene;
        [SerializeField] private GameObject UI;

        public void OnPlayerDeath()
        {
            Debug.Log("☠️ Player died! Attempting reload...");

            if (_loadCharacter == null || _loadCharacterData == null || _hotbarStats == null)
            {
                Debug.LogError("❌ PlayerDeathHandler: One or more references are missing!");
                return;
            }
            deathscene.SetActive(true);
            UI.SetActive(false);
            _loadCharacter.LoadCharacterFromSave();
            _loadCharacterData.LoadCharacterSaveData();
            _hotbarStats.setAttributeManager();
            _levelManager.UpdateAllUI();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = Vector3.zero; // or your respawn position
            }
        }
    }
}
