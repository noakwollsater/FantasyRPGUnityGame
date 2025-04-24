using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject IngameUI;

        [SerializeField] private GameObject radialPanel;
        [SerializeField] private GameObject settingsPanel;

        [SerializeField] private CameraController cameraController;
        [SerializeField] private SaveGameManager saveGameManager;

        private CharacterSaveData data;

        private UltimateCharacterLocomotion characterLocomotion;
        private GameObject Character;

        private bool isMainMenuOpen = false;
        private bool isInventoryOpen = false;

        void Update()
        {
            // Ensure characterLocomotion is assigned
            if (characterLocomotion == null)
            {
                Character = GameObject.FindGameObjectWithTag("Player");
                if (Character != null)
                {
                    characterLocomotion = Character.GetComponent<UltimateCharacterLocomotion>();
                }
            }

            // Toggle Main Menu with Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isMainMenuOpen)
                {
                    OpenMainMenu();
                }
                else
                {
                    ResumeGame();
                }
            }

            // Toggle Inventory with I key
            if (Input.GetKeyDown(KeyCode.I))
            {
                isInventoryOpen = !isInventoryOpen;
                inventoryPanel.SetActive(isInventoryOpen);

                if (characterLocomotion != null)
                    characterLocomotion.enabled = !isInventoryOpen;

                if (cameraController != null)
                    cameraController.enabled = !isInventoryOpen;

                Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isInventoryOpen;

                // Close main menu if inventory opened
                if (isInventoryOpen && isMainMenuOpen)
                {
                    isMainMenuOpen = false;
                    mainMenuPanel.SetActive(false);
                    IngameUI.SetActive(false);
                }
            }

            // Close Settings if open

            if (settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = isInventoryOpen;
                settingsPanel.SetActive(false);
                radialPanel.SetActive(true);
            }
        }

        private void UpdateCursorState()
        {
            if (isMainMenuOpen || isInventoryOpen || settingsPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }


        public void ResumeGame()
        {
            isMainMenuOpen = false;
            mainMenuPanel.SetActive(false);
            radialPanel.SetActive(true);
            settingsPanel.SetActive(false);
            IngameUI.SetActive(true);

            if (characterLocomotion != null)
                characterLocomotion.enabled = true;

            if (cameraController != null)
                cameraController.enabled = true;
        }

        private void OpenMainMenu()
        {
            isMainMenuOpen = true;
            mainMenuPanel.SetActive(true);
            IngameUI.SetActive(false);

            if (characterLocomotion != null)
                characterLocomotion.enabled = false;

            if (cameraController != null)
                cameraController.enabled = false;

            // Close inventory if open
            if (isInventoryOpen)
            {
                isInventoryOpen = false;
                inventoryPanel.SetActive(false);
            }
        }

        public void QuitGame()
        {
            saveGameManager.SaveCharacterData();
            saveGameManager.SaveGameData("Start of the Journey", "Heimdal", SaveType.Manual, "00:00", Character.transform);
            PlayerPrefs.SetInt("ReturnToMainMenu", 1);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }

        public void OpenSettings()
        {
            settingsPanel.SetActive(true);
            radialPanel.SetActive(false);
        }

        public void CloseSettings()
        {
            settingsPanel.SetActive(false);
            radialPanel.SetActive(true);
        }
    }
}
