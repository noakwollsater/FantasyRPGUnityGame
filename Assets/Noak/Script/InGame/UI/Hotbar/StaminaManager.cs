using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Character.Abilities;

namespace Unity.FantasyKingdom
{
    public class StaminaManager : MonoBehaviour
    {
        private UltimateCharacterLocomotion _locomotion;
        private AttributeManager _attributeManager;
        private Attribute _staminaAttribute;
        private SpeedChange _speedChangeAbility;

        private bool _jumpRequested = false;
        private float _jumpTimer = 0f;
        private const float JumpStaminaDelay = 2f; // 2 seconds delay after jump before draining stamina

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

            HandleRunning();
            HandleJumping();
        }

        private void HandleRunning()
        {
            bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            if (isMoving && isRunning)
            {
                DrainStamina(10);

                if (_staminaAttribute.Value <= 2)
                {
                    _speedChangeAbility.MaxSpeedChangeValue = 1f; // Slow down when out of stamina
                }
            }
        }

        private void HandleJumping()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_jumpRequested)
            {
                _jumpRequested = true;
                _jumpTimer = 0f;
            }

            if (_jumpRequested)
            {
                _jumpTimer += Time.deltaTime;
                if (_jumpTimer >= JumpStaminaDelay)
                {
                    DrainStamina(10); // Drain stamina after delay
                    _jumpRequested = false; // Reset jump request
                }
            }
        }

        private void DrainStamina(int value)
        {
            _staminaAttribute.Value -= value * Time.deltaTime;
            if (_staminaAttribute.Value < 0)
            {
                _staminaAttribute.Value = 0;
            }
        }
    }
}
