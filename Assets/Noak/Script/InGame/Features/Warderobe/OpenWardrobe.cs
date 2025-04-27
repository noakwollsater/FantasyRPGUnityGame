using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OpenWardrobe : MonoBehaviour
{
    UltimateCharacterLocomotion characterLocomotion;
    CharacterRotation characterRotation;

    [SerializeField] private Camera camera;
    [SerializeField] private Camera warderobeCamera;
    [SerializeField] private GameObject UI;

    private void OnEnable()
    {
        FindComponents();
        PositionCamera();
    }
    private void OnDisable()
    {
        if (characterLocomotion != null)
        {
            characterLocomotion.enabled = true;
        }
        if (characterRotation != null)
        {
            characterRotation.enabled = false;
        }
        camera.enabled = true;
        warderobeCamera.enabled = false;
        UI.SetActive(true);
    }   

    private void FindComponents()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        characterLocomotion = character.GetComponent<UltimateCharacterLocomotion>();
        characterRotation = character.GetComponent<CharacterRotation>();
        if (characterLocomotion == null)
        {
            Debug.LogError("Character Locomotion not found on the player.");
        }
        if (characterRotation == null)
        {
            Debug.LogError("Character Rotation not found on the player.");
        }
        characterLocomotion.enabled = false;
        characterRotation.enabled = true;
        camera.enabled = false;
        warderobeCamera.enabled = true;
        UI.SetActive(false);
    }

    private void PositionCamera()
    {
        // Calculate the position and rotation for the wardrobe camera
        Vector3 targetPosition = characterLocomotion.transform.position + new Vector3(0, 1.5f, -2);
        Quaternion targetRotation = Quaternion.Euler(0, characterLocomotion.transform.eulerAngles.y, 0);

        // Set the camera's position and rotation
        warderobeCamera.transform.position = targetPosition;
        warderobeCamera.transform.rotation = targetRotation;
    }
}