using Opsive.UltimateCharacterController.Camera;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float interactionRange = 3f;
    public Camera cam;
    public InteractionPromptUI promptUI;

    private IInteractable currentInteractable;

    void Start()
    {
        cam = FindAnyObjectByType<Camera>();
        if (promptUI == null)
        {
            promptUI = FindObjectOfType<InteractionPromptUI>();
        }
    }

    void Update()
    {
        CheckForInteractable();

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                string prompt = interactable.GetInteractionPrompt();
                if (!string.IsNullOrEmpty(prompt))
                {
                    currentInteractable = interactable;
                    promptUI.Show(prompt);
                    return;
                }
            }
        }

        currentInteractable = null;
        promptUI.Hide();
    }
}