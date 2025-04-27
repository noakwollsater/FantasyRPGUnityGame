using Opsive.UltimateCharacterController.Camera;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class WeaponWheel : MonoBehaviour
    {
        [SerializeField] private GameObject weaponWheelPanel;
        [SerializeField] private CameraControllerHandler cameraController;

        private void Start()
        {
            if (weaponWheelPanel != null)
            {
                weaponWheelPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                ShowWeaponWheel();
            }
            else
            {
                HideWeaponWheel();
            }
        }

        private void ShowWeaponWheel()
        {
            if (weaponWheelPanel != null && !weaponWheelPanel.activeSelf)
            {
                cameraController.m_BlockLookInput = true;
                weaponWheelPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void HideWeaponWheel()
        {
            if (weaponWheelPanel != null && weaponWheelPanel.activeSelf)
            {
                cameraController.m_BlockLookInput = false;
                weaponWheelPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
