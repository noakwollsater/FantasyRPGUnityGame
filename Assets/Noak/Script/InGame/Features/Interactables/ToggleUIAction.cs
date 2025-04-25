using UnityEngine;

public class ToggleUIAction : InteractionAction
{
    public UIController uiController;

    public override void Execute()
    {
        if (uiController != null)
            uiController.ToggleUI();
    }
}