using Opsive.UltimateCharacterController.Camera;
using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class ZoomFunction : MonoBehaviour
    {
        [SerializeField] private CameraController cameraController;
        [SerializeField] private GameObject zoomPanel;

        private void Start()
        {
            zoomPanel.SetActive(false);
        }

        private void Update()
        {
            if (cameraController.m_Zoom == true)
            {
                zoomPanel.SetActive(true);
            }
            else
            {
                zoomPanel.SetActive(false);
            }
        }

        public void EnableZoom()
        {
            cameraController.m_CanZoom = true;
        }
    }
}
