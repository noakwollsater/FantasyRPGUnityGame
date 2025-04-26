using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace Unity.FantasyKingdom
{
    public class StaminaManager : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField] private float staminaDrainPerSecond = 10f;
        [SerializeField] private float staminaRegenPerSecond = 5f;

        private UltimateCharacterLocomotion _locomotion;
        private AttributeManager _attributeManager;
        private Attribute _staminaAttribute;
        private SpeedChange _speedChangeAbility;

        private void Awake()
        {
            _locomotion = GetComponent<UltimateCharacterLocomotion>();
            _attributeManager = GetComponent<AttributeManager>();

            if (_locomotion != null)
            {
                _speedChangeAbility = _locomotion.GetAbility<SpeedChange>();
            }

            if (_attributeManager != null)
            {
                _staminaAttribute = _attributeManager.GetAttribute("Stamina");
            }
        }

        private void Update()
        {
            if (_staminaAttribute == null || _speedChangeAbility == null) return;

            bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            if (isMoving && isRunning)
            {
                DrainStamina();

                if (_staminaAttribute.Value <= 2)
                {
                    // Stamina is very low, slow down the player.
                    _speedChangeAbility.MaxSpeedChangeValue = 1f;
                }
            }
            if (_staminaAttribute.Value > 2)
            {
                // Restore sprinting speed when stamina is recovered.
                _speedChangeAbility.MaxSpeedChangeValue = 0.3f * _locomotion.MotorAcceleration.magnitude + 2f;
            }
        }

        private void DrainStamina()
        {
            _staminaAttribute.Value -= staminaDrainPerSecond * Time.deltaTime;
            if (_staminaAttribute.Value < 0)
            {
                _staminaAttribute.Value = 0;
            }
        }
    }
}
