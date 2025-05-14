using UnityEngine;
using TMPro;

public class SkillTree : MonoBehaviour
{
    public SkillNode[] skillNodes;
    public UnityEngine.UI.Button treeIcon;

    private void Start()
    {
        foreach (var node in skillNodes)
        {
            Debug.Log($"{node.name} depthFromStart = {node.depthFromStart} (isStartingSkill = {node.isStartingSkill})");
        }

        Debug.Log($"SkillTree contains {skillNodes.Length} nodes.");
        foreach (var node in skillNodes)
        {
            Debug.Log($"- {node.name}");
        }

        CheckTreeCompletion();
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
