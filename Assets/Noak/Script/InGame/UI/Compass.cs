using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class Compass : MonoBehaviour
    {
        public Transform viewDirection;
        public RectTransform compassElement;
        public float compassSize;

        private void LateUpdate()
        {
            Vector3 forwardVector = Vector3.ProjectOnPlane(
                viewDirection.forward, Vector3.up).normalized;
            float forwardSignedAngle = Vector3.SignedAngle(
                forwardVector, Vector3.forward, Vector3.up);
            float compassOffset = forwardSignedAngle / 180f * compassSize;
            compassElement.anchoredPosition = new Vector2(compassOffset, 0f);
        }
    }
}
