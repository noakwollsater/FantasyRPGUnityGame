using Opsive.UltimateCharacterController.Camera;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject uiPanel;
    public Transform player;
    public Transform targetObject;
    public float maxDistance = 4f;
    public CameraController cameraController; // Assuming you have a CameraController script to manage camera behavior

    private bool isUIOpen = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isUIOpen && Vector3.Distance(player.position, targetObject.position) > maxDistance)
        {
            CloseUI();
        }

        if (isUIOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
        }
    }

    public void ToggleUI()
    {
        if (isUIOpen)
            CloseUI();
        else
            OpenUI();
    }

    public void OpenUI()
    {
        uiPanel.SetActive(true);
        isUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cameraController.enabled = false; // Disable camera controller if you have one
    }

    public void CloseUI()
    {
        uiPanel.SetActive(false);
        isUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraController.enabled = true; // Re-enable camera controller if you have one

        // Återaktivera spelarkontroll här om du stängt av den
        // t.ex. FirstPersonController.enabled = true;
    }
}
