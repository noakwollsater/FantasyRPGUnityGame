using Opsive.UltimateCharacterController.Traits;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FantasyKingdom
{
    public class CustomCharacterRespawner : CharacterRespawner
    {
        private GameObject deathPanel;
        private SaveGameManager saveGameManager;
        CharacterSaveData data;
        private bool waitingForInput = false;
        private Vector3 respawnPosition;
        private Quaternion respawnRotation;
        private bool respawnTransformChange;

        private void Awake()
        {
            GameObject UI = GameObject.Find("UI");
            if (UI == null)
            {
                Debug.LogError("❌ UI GameObject not found!");
                return;
            }
            GameObject Monitor = UI.transform.Find("Monitors").gameObject;
            if (Monitor == null)
            {
                Debug.LogError("❌ Monitor GameObject not found!");
                return;
            }
            deathPanel = Monitor.transform.Find("Screen_FantasyMenus_Death_01").gameObject;
            saveGameManager = FindObjectOfType<SaveGameManager>();

            if (deathPanel == null)
            {
                Debug.LogError("❌ Death panel not found!");
            }
            if (saveGameManager == null)
            {
                Debug.LogError("❌ SaveGameManager not found!");
            }
        }


        private void Update()
        {
            if (!waitingForInput) return;

            if (Input.GetKeyDown(KeyCode.F))
            {
                data.isDead = false;
                Debug.Log("🔄 Player chose to respawn!");

                if (deathPanel != null)
                    deathPanel.SetActive(false);

                waitingForInput = false;
                base.Respawn(respawnPosition, respawnRotation, respawnTransformChange);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("🚪 Player chose to quit!");
                
                GameObject characterGameObject = GameObject.FindGameObjectWithTag("Player");

                data.isDead = true;
                saveGameManager.SaveCharacterData();
                saveGameManager.SaveGameData("Start of the Journey", "Heimdal", SaveType.Manual, "00:00", characterGameObject.transform);
                PlayerPrefs.SetInt("ReturnToMainMenu", 1);
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }
        }

        public override void Respawn(Vector3 position, Quaternion rotation, bool transformChange)
        {
            Debug.Log("🛑 Pausing respawn - waiting for player input!");

            if (deathPanel != null)
                deathPanel.SetActive(true);

            waitingForInput = true;

            // Save respawn data to use later
            respawnPosition = position;
            respawnRotation = rotation;
            respawnTransformChange = transformChange;
        }
    }
}
