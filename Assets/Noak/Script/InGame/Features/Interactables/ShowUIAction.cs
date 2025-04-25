using UnityEngine;

public class ShowUIAction : InteractionAction
{
    public GameObject uiPanel;

    public override void Execute()
    {
        if (uiPanel != null)
            uiPanel.SetActive(true);
    }
}
