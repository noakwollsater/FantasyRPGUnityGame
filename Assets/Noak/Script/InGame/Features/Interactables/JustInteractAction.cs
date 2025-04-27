using UnityEngine;

public class JustInteractAction : InteractionAction
{
	public override void Execute()
	{
		// This action does nothing, but it can be used to trigger an interaction without any specific action.
        // For example, it can be used to trigger a dialogue or a cutscene without any specific action.
        Debug.Log("JustInteractAction executed.");
        // You can add any additional logic here if needed.
        // For example, you could trigger a sound effect or a visual effect.
        // But in this case, we are just logging the execution of the action.
	}
}