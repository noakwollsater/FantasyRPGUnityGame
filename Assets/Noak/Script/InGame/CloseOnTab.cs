using Opsive.UltimateCharacterController.Camera;
using UnityEngine;

public class CloseOnTab : MonoBehaviour
{
    public CameraController cameraController; // Assuming you have a CameraController script to manage camera behavior
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            gameObject.SetActive(false);
            cameraController.enabled = true; // Re-enable camera controller if you have one
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
