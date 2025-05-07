using MalbersAnimations;
using MalbersAnimations.Scriptables;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class WeaponWheel : MonoBehaviour
    {
        [SerializeField] private GameObject weaponWheelPanel;
        [SerializeField] private ThirdPersonFollowTarget camera;

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
            if (Input.GetKey(KeyCode.Tab))
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
                camera.YMultiplier = 0;
                camera.XMultiplier = 0;
                weaponWheelPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void HideWeaponWheel()
        {
            if (weaponWheelPanel != null && weaponWheelPanel.activeSelf)
            {
                camera.YMultiplier = 1;
                camera.XMultiplier = 1;
                weaponWheelPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
