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
        private Jump _jumpAbility;
        private float _lastJumpTime;
        private const float JumpCooldown = 1f; // 2 seconds cooldown
        private const float MinStaminaToJump = 2f; // Minimum stamina needed to jump

        private Attribute _hungerAttribute;
        private Attribute _thirstAttribute;

        private void Awake()
        {
            _locomotion = GetComponent<UltimateCharacterLocomotion>();
            _attributeManager = GetComponent<AttributeManager>();

            if (_locomotion != null)
            {
                _speedChangeAbility = _locomotion.GetAbility<SpeedChange>();
                _jumpAbility = _locomotion.GetAbility<Jump>();
            }

            if (_attributeManager != null)
            {
                _staminaAttribute = _attributeManager.GetAttribute("Stamina");
                _hungerAttribute = _attributeManager.GetAttribute("Hunger");
                _thirstAttribute = _attributeManager.GetAttribute("Thirst");
            }
        }

        private void Update()
        {
            if (_staminaAttribute == null || _speedChangeAbility == null) return;

            HandleRunning();
            HandleJumping();
            HandleSpeedRecovery();
            HandlingJumpingRecovery();
        }

        private void HandleRunning()
        {
            bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            if (isMoving && isRunning)
            {
                DrainStamina(10);
                HandleHungerAndThirst(); // Drain hunger and thirst when running

                if (_staminaAttribute.Value <= 2)
                {
                    _speedChangeAbility.MaxSpeedChangeValue = 1f; // Slow down when out of stamina
                }
            }
        }

        private void HandleJumping()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - _lastJumpTime >= JumpCooldown) // Check cooldown
                {
                    if (_staminaAttribute.Value >= MinStaminaToJump)
                    {
                        _jumpAbility.StartAbility(); // Actually jump
                        HandleHungerAndThirst(); // Drain hunger and thirst when jumping
                        DrainStaminaInstant(10); // Immediately drain stamina
                        _lastJumpTime = Time.time; // Reset cooldown timer
                    }
                    else
                    {
                        _jumpAbility.Force = 0.05f; 
                    }
                }
                else
                {
                    Debug.Log("Jump is on cooldown!");
                }
            }
        }

        private void HandleHungerAndThirst()
        {
            if (_hungerAttribute != null && _thirstAttribute != null)
            {
                DrainHunger();
                DrainThirst();
            }
        }

        private void DrainHunger()
        {
            if (_hungerAttribute.Value > 0)
            {
                _hungerAttribute.Value -= 0.5f * Time.deltaTime;
            }
        }

        private void DrainThirst()
        {
            if (_thirstAttribute.Value > 0)
            {
                _thirstAttribute.Value -= 0.5f * Time.deltaTime;
            }
        }

        private void HandleSpeedRecovery()
        {
            if (_staminaAttribute.Value > 2)
            {
                _speedChangeAbility.MaxSpeedChangeValue = 0.3f * _locomotion.MotorAcceleration.magnitude + 2f; // Restore running speed
            }
        }

        private void HandlingJumpingRecovery()
        {
            if (_staminaAttribute.Value > 2)
            {
                _jumpAbility.Force = 0.2f; // Restore jump height
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

        private void DrainStaminaInstant(int value)
        {
            _staminaAttribute.Value -= value;
            if (_staminaAttribute.Value < 0)
            {
                _staminaAttribute.Value = 0;
            }
        }

    }
}
