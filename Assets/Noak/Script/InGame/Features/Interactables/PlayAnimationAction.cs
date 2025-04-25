using UnityEngine;

public class PlayAnimationAction : InteractionAction
{
    public Animator animator;
    public string animationTrigger;

    public override void Execute()
    {
        if (animator != null)
            animator.SetTrigger(animationTrigger);
    }
}
