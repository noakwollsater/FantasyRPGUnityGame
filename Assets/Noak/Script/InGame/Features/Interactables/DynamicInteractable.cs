using UnityEngine;

public class DynamicInteractable : MonoBehaviour, IInteractable
{
    public string interactionText = "Press F to interact";
    public InteractionAction[] actions;

    private bool hasInteracted = false;

    public string GetInteractionPrompt()
    {
        return hasInteracted ? "" : interactionText;
    }

    public void Interact()
    {
        foreach (var action in actions)
        {
            action.Execute();
        }

        hasInteracted = true; // om du vill att det bara sker en gång
    }
}
