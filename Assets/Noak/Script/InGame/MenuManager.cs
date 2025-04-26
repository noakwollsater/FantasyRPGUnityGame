using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class MenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject ingameUI;
        [SerializeField] private GameObject radialPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Dependencies")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SaveGameManager saveGameManager;

        private UltimateCharacterLocomotion characterLocomotion;
        private GameObject characterGameObject;

        private bool isMainMenuOpen;
        private bool isInventoryOpen;

        private void Update()
        {
            EnsureCharacterLocomotionInitialized();

            HandleEscapeKey();
            HandleInventoryToggle();
            HandleSettingsToggle();
        }

        private void EnsureCharacterLocomotionInitialized()
        {
            if (characterLocomotion == null)
            {
                characterGameObject = GameObject.FindGameObjectWithTag("Player");
                if (characterGameObject != null)
                {
                    characterLocomotion = characterGameObject.GetComponent<UltimateCharacterLocomotion>();
                }
            }
        }

        private void HandleEscapeKey()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isMainMenuOpen)
                    ResumeGame();
                else
                    OpenMainMenu();

                UpdateCursorState();
            }
        }

        private void HandleInventoryToggle()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
        }

        private void ToggleInventory()
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);

            SetCharacterControlActive(!isInventoryOpen);
            UpdateCursorState();

            // Close main menu if inventory opened
            if (isInventoryOpen && isMainMenuOpen)
            {
                isMainMenuOpen = false;
                mainMenuPanel.SetActive(false);
                ingameUI.SetActive(false);
            }
        }

        private void HandleSettingsToggle()
        {
            if (settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab))
            {
                settingsPanel.SetActive(false);
                radialPanel.SetActive(true);
                UpdateCursorState();
            }
        }

        private void SetCharacterControlActive(bool isActive)
        {
            if (characterLocomotion != null)
                characterLocomotion.enabled = isActive;

            if (cameraController != null)
                cameraController.enabled = isActive;
        }

        private void UpdateCursorState()
        {
            if (isMainMenuOpen || isInventoryOpen || settingsPanel.activeSelf)
                UnlockCursor();
            else
                LockCursor();
        }

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            isMainMenuOpen = false;
            mainMenuPanel.SetActive(false);
            radialPanel.SetActive(true);
            settingsPanel.SetActive(false);
            ingameUI.SetActive(true);

            SetCharacterControlActive(true);
            LockCursor();
        }

        private void OpenMainMenu()
        {
            isMainMenuOpen = true;
            mainMenuPanel.SetActive(true);
            ingameUI.SetActive(false);

            SetCharacterControlActive(false);
            if (isInventoryOpen)
            {
                isInventoryOpen = false;
                inventoryPanel.SetActive(false);
            }
        }

        public void QuitGame()
        {
            saveGameManager.SaveCharacterData();
            saveGameManager.SaveGameData("Start of the Journey", "Heimdal", SaveType.Manual, "00:00", characterGameObject.transform);
            PlayerPrefs.SetInt("ReturnToMainMenu", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        public void OpenSettings()
        {
            settingsPanel.SetActive(true);
            radialPanel.SetActive(false);
            UpdateCursorState();
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
            radialPanel.SetActive(true);
            UpdateCursorState();
        }
    }
}
