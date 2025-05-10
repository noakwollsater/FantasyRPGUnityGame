using MalbersAnimations;
using MalbersAnimations.Scriptables;
using UnityEngine;
using UnityEngine.Rendering;

namespace Unity.FantasyKingdom
{
    public class WeaponWheel : MonoBehaviour
    {
        [SerializeField] private GameObject weaponWheelPanel;
        [SerializeField] private ThirdPersonFollowTarget camera;

        public GameObject CMThirdPersonCamera;

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
            if(camera == null)
            {
                GameObject cameraCM2 = GameObject.Find("Cameras CM2");
                CMThirdPersonCamera = cameraCM2.transform.Find("CM Third Person Main (New Input)").gameObject;
                camera = CMThirdPersonCamera.GetComponent<ThirdPersonFollowTarget>();
            }

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
