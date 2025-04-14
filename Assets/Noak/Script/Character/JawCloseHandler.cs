using UnityEngine;

// Ensures the jaw stays closed by locking its rotation every frame.
[RequireComponent(typeof(Animator))]
public class JawCloseHandler : MonoBehaviour
{
    // Reference to the jaw bone transform.
    private Transform _jawBone;

    // Rotation value representing the closed position of the jaw.
    private Quaternion _closedJawRotation;

    // Called when the script instance is being loaded. Caches jaw bone and its closed rotation.
    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        _jawBone = animator.GetBoneTransform(HumanBodyBones.Jaw);
        _closedJawRotation = _jawBone.localRotation;
    }

    // Called every frame after all Update calls. Locks jaw bone to the closed rotation.
    private void LateUpdate()
    {
        _jawBone.localRotation = _closedJawRotation;
    }
}