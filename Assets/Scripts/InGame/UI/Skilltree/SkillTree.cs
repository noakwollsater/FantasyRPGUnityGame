using UnityEngine;
using TMPro;
using System.Collections;

public class SkillTree : MonoBehaviour
{
    [Header("Identifier")]
    public string skillTreeName; // t.ex. "Combat", "Athletics", "Attributes"

    public SkillNode[] skillNodes;
    public UnityEngine.UI.Button treeIcon;

    private void OnEnable()
    {
        StartCoroutine(InitializeSkillNodes());
    }

    private IEnumerator InitializeSkillNodes()
    {
        yield return new WaitForEndOfFrame(); // Vänta tills allt är spawna/initierat

        CheckTreeCompletion(); // Kan också köras här
    }


    public void CheckTreeCompletion()
    {
        foreach (SkillNode node in skillNodes)
        {
            if (!node.IsUnlocked)
                return;
        }

        var animator = treeIcon.animator;
        if (animator != null)
        {
            animator.ResetTrigger(treeIcon.animationTriggers.normalTrigger);
            animator.ResetTrigger(treeIcon.animationTriggers.highlightedTrigger);
            animator.ResetTrigger(treeIcon.animationTriggers.disabledTrigger);
            animator.SetTrigger(treeIcon.animationTriggers.selectedTrigger);
        }

        treeIcon.transition = UnityEngine.UI.Selectable.Transition.None;
        treeIcon.interactable = false;
    }
}
