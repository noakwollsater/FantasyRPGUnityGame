using UnityEngine;

public class DynamicInteractable : MonoBehaviour, IInteractable
{
    public string interactionText;
    public InteractionAction[] actions;

    public bool allowMultipleInteractions = true; // ✅ NYTT
    private bool hasInteracted = false;

    public string GetInteractionPrompt()
    {
        if (!allowMultipleInteractions && hasInteracted)
            return "";

        return interactionText;
    }

    public void Interact()
    {
        foreach (var action in actions)
        {
            action.Execute();
        }

        if (!allowMultipleInteractions)
            hasInteracted = true;
    }
}
