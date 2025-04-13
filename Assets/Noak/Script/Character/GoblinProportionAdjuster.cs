using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GoblinProportionAdjuster : MonoBehaviour
{
    private Animator animator;

    // Justerbara proportioner i inspectorn
    [Header("Head Scale")]
    public Vector3 headScale = new Vector3(1.4f, 1.4f, 1.4f);

    [Header("Arm Scale")]
    public Vector3 armScale = new Vector3(0.7f, 0.7f, 0.7f);

    [Header("Leg Scale")]
    public Vector3 legScale = new Vector3(0.8f, 0.8f, 0.8f);

    [Header("Spine Scale")]
    public Vector3 spineScale = new Vector3(0.85f, 0.85f, 0.85f);

    void Start()
    {
        animator = GetComponent<Animator>();

        AdjustProportions();
    }

    void AdjustProportions()
    {
        // Huvud
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        if (head) head.localScale = headScale;

        // Armar
        Transform leftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        Transform rightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        if (leftArm) leftArm.localScale = armScale;
        if (rightArm) rightArm.localScale = armScale;

        // Ben
        Transform leftLeg = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        Transform rightLeg = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        if (leftLeg) leftLeg.localScale = legScale;
        if (rightLeg) rightLeg.localScale = legScale;

        // Rygg
        Transform spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        if (spine) spine.localScale = spineScale;
    }
}
