using MalbersAnimations;
using MalbersAnimations.Controller;
using MalbersAnimations.InputSystem;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private GameObject skilltreePanel;

        private ThirdPersonFollowTarget camera;
        private MAnimal playerInput;

        [Header("Dependencies")]
        [SerializeField] private SaveGameManager saveGameManager;

        private GameObject characterGameObject;

        private bool isMainMenuOpen;
        private bool isInventoryOpen;
        private bool isSettingsOpen;
        private bool isSkilltreeOpen;

        private void Update()
        {
            EnsureCharacterInitialized();

            HandleEscapeKey();
            HandleInventoryToggle();
            HandleSettingsToggle();
            HandleSkilltreeToggle();

            if (camera == null)
            {
                GameObject cameraCM2 = GameObject.Find("Cameras CM2");
                GameObject CMThirdPersonCamera = cameraCM2.transform.Find("CM Third Person Main (New Input)").gameObject;
                camera = CMThirdPersonCamera.GetComponent<ThirdPersonFollowTarget>();
            }

            if(saveGameManager == null)
            {
                saveGameManager = GameObject.FindGameObjectWithTag("HUD").GetComponent<SaveGameManager>();
            }
        }

        private void EnsureCharacterInitialized()
        {
            if (characterGameObject == null)
            {
                characterGameObject = GameObject.FindGameObjectWithTag("Player");
                playerInput = characterGameObject.GetComponent<MAnimal>();
            }
            if (ingameUI == null)
            {
                ingameUI = GameObject.FindGameObjectWithTag("IngameUI");
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

        private void HandleSettingsToggle()
        {
            if (settingsPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab))
            {
                settingsPanel.SetActive(false);
                radialPanel.SetActive(true);
                UpdateCursorState();
            }
        }
        private void HandleInventoryToggle()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                isInventoryOpen = !isInventoryOpen;
                inventoryPanel.SetActive(isInventoryOpen);
                radialPanel.SetActive(!isInventoryOpen);
                UpdateCursorState();
            }
        }
        private void HandleSkilltreeToggle()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                isSkilltreeOpen = !isSkilltreeOpen;
                skilltreePanel.SetActive(isSkilltreeOpen);
                radialPanel.SetActive(!isSkilltreeOpen);
                UpdateCursorState();
            }
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

        public void OnSaveBtn()
        {
            saveGameManager.SaveCharacterData();
            saveGameManager.SaveGameData("Start of the Journey", "Heimdal", SaveType.Manual, "00:00", characterGameObject.transform);
        }
        public void ResumeGame()
        {
            isMainMenuOpen = false;
            mainMenuPanel.SetActive(false);
            radialPanel.SetActive(true);
            settingsPanel.SetActive(false);
            skilltreePanel.SetActive(false);
            ingameUI.SetActive(true);
            camera.YMultiplier = 1;
            camera.XMultiplier = 1;
            playerInput.enabled = true;

            LockCursor();
        }
        private void OpenMainMenu()
        {
            isMainMenuOpen = true;
            mainMenuPanel.SetActive(true);
            ingameUI.SetActive(false);
            camera.YMultiplier = 0;
            camera.XMultiplier = 0;
            playerInput.enabled = false;

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
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
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
        public void OpenSkilltree()
        {
            skilltreePanel.SetActive(true);
            radialPanel.SetActive(false);
            UpdateCursorState();
        }
        public void CloseSkilltree()
        {
            skilltreePanel.SetActive(false);
            radialPanel.SetActive(true);
            isSkilltreeOpen = false; // <-- fixen
            UpdateCursorState();
        }
        public void OpenInventory()
        {
            inventoryPanel.SetActive(true);
            radialPanel.SetActive(false);
            UpdateCursorState();
        }
        public void CloseInventory()
        {
            inventoryPanel.SetActive(false);
            radialPanel.SetActive(true);
            UpdateCursorState();
        }
    }
}
