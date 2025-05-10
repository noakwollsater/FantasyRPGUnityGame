using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class AutoSave : MonoBehaviour
    {
        [SerializeField] private SaveGameManager saveGameManager;
        [SerializeField] private GameObject autoSavePanel;
        [SerializeField] private float saveInterval = 300f; // var femte minut
        [SerializeField] private float savePopupDuration = 1.5f;

        private float saveTimer;

        private GameObject character;

        void Start()
        {
            saveTimer = saveInterval;

            if (autoSavePanel != null)
                autoSavePanel.SetActive(false);

            if (saveGameManager == null)
                Debug.LogError("AutoSave: SaveGameManager is not assigned!");
        }

        void Update()
        {
            saveTimer -= Time.deltaTime;

            if (saveTimer <= 0f)
            {
                saveTimer = saveInterval;
                TriggerAutoSave();
            }
        }

        private void TriggerAutoSave()
        {
            character = GameObject.FindGameObjectWithTag("Player");

            if (saveGameManager != null)
            {
                string chapter = "Chapter1";
                string area = "Forest";
                string timeOfDay = "Noon";
                Transform characterTransform = character.transform;

                saveGameManager.SaveGameData(chapter, area, SaveType.AutoSave, timeOfDay, characterTransform);
                saveGameManager.SaveCharacterData();

                if (autoSavePanel != null)
                {
                    autoSavePanel.SetActive(true);
                    Invoke(nameof(HideSavePanel), savePopupDuration);
                }
            }
        }

        private void HideSavePanel()
        {
            if (autoSavePanel != null)
                autoSavePanel.SetActive(false);
        }
    }
}
