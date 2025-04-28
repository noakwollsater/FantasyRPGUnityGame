using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OpenWardrobe : MonoBehaviour
{
    UltimateCharacterLocomotion characterLocomotion;
    CharacterRotation characterRotation;

    [SerializeField] private GameObject camera;
    [SerializeField] private Camera warderobeCamera;
    [SerializeField] private GameObject UI;
    [SerializeField] private Light light;

    public GameObject child;

    private Animator animator;
    private RuntimeAnimatorController idleAnimator;
    private RuntimeAnimatorController playerAnimator;

    [SerializeField] private Vector3 cameraOffset = new Vector3(1.4f, 1.5f, 1.7f);
    [SerializeField] private Vector3 cameraEulerRotation = new Vector3(12, -160, 0);

    private void OnEnable()
    {
        FindAnimators();
        FindComponents();
        PositionCamera();
    }

    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.P))
        {
            PositionCamera();
        }
    }

    private void OnDisable()
    {
        CloseWardrobe();
    }

    private void FindComponents()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        GameObject child = character.transform.Find("Player").gameObject;
        animator = child.GetComponent<Animator>();
        characterLocomotion = character.GetComponent<UltimateCharacterLocomotion>();
        characterRotation = character.GetComponent<CharacterRotation>();
        animator.runtimeAnimatorController = idleAnimator;
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
        camera.active = false;
        warderobeCamera.enabled = true;
        UI.SetActive(false);
        light.enabled = true;
    }

    private void CloseWardrobe()
    {
        if (characterLocomotion != null)
        {
            characterLocomotion.enabled = true;
        }
        if (characterRotation != null)
        {
            characterRotation.enabled = false;
        }
        if (playerAnimator != null)
        {
            animator.runtimeAnimatorController = playerAnimator;
        }
        camera.active = true;
        UI.SetActive(true);
        warderobeCamera.enabled = false;
        animator.runtimeAnimatorController = playerAnimator;
        light.enabled = false;
    }

    private void PositionCamera()
    {
        warderobeCamera.transform.position = characterLocomotion.transform.position + cameraOffset;
        warderobeCamera.transform.eulerAngles = cameraEulerRotation;
    }



    private void FindAnimators()
    {
        idleAnimator = Resources.Load<RuntimeAnimatorController>("IdleState");
        if (idleAnimator == null)
        {
            Debug.LogError("IdleState animator controller not found.");
            return;
        }
        playerAnimator = Resources.Load<RuntimeAnimatorController>("Demo");
        if (playerAnimator == null)
        {
            Debug.LogError("Demo animator controller not found.");
            return;
        }
    }
}