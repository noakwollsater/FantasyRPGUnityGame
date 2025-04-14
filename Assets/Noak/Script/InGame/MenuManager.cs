using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject inventoryPanel;

        [SerializeField] private CameraController cameraController;

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
                }
            }
        }

        public void ResumeGame()
        {
            isMainMenuOpen = false;
            mainMenuPanel.SetActive(false);

            if (characterLocomotion != null)
                characterLocomotion.enabled = true;

            if (cameraController != null)
                cameraController.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OpenMainMenu()
        {
            isMainMenuOpen = true;
            mainMenuPanel.SetActive(true);

            if (characterLocomotion != null)
                characterLocomotion.enabled = false;

            if (cameraController != null)
                cameraController.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Close inventory if open
            if (isInventoryOpen)
            {
                isInventoryOpen = false;
                inventoryPanel.SetActive(false);
            }
        }
    }
}
