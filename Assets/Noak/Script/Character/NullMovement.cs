using Opsive.UltimateCharacterController.Character.MovementTypes;
using UnityEngine;

public class NullMovement : MovementType
{
    public override bool FirstPersonPerspective => false;

    public override float GetDeltaYawRotation(float characterHorizontalMovement, float characterForwardMovement, float cameraHorizontalMovement, float cameraVerticalMovement)
    {
        return 0f;
    }

    public override Vector2 GetInputVector(Vector2 inputVector)
    {
        return inputVector; // Eller return Vector2.zero;
    }
}
