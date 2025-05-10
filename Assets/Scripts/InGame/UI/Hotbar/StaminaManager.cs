using UnityEngine;
using Opsive.UltimateCharacterController.Traits;
using MalbersAnimations;

namespace Unity.FantasyKingdom
{
    public class StaminaManager : MonoBehaviour
    {
        private AttributeManager _attributeManager;
        private Attribute _staminaAttribute;
        private Attribute _hungerAttribute;
        private Attribute _thirstAttribute;
        private Stats _malbersStats;
        private Stat _malbersStamina;

        private void Awake()
        {
            _attributeManager = GetComponent<AttributeManager>();
            _malbersStats = GetComponent<Stats>();

            if (_attributeManager != null)
            {
                _staminaAttribute = _attributeManager.GetAttribute("Stamina");
                _hungerAttribute = _attributeManager.GetAttribute("Hunger");
                _thirstAttribute = _attributeManager.GetAttribute("Thirst");
            }

            if (_malbersStats != null)
            {
                _malbersStamina = _malbersStats.stats.Find(x => x.Name == "Stamina");
                if (_staminaAttribute != null && _malbersStamina != null)
                {
                    _malbersStamina.Value = _staminaAttribute.Value;
                    _malbersStamina.MaxValue = _staminaAttribute.MaxValue;
                }
            }
        }

        private void Update()
        {
            if (_staminaAttribute == null || _malbersStamina == null) return;

            HandleRunning();

            // Synka Malbers stamina efter eventuell förändring
            SyncStaminaToMalbers();
        }

        private void HandleRunning()
        {
            bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            if (isMoving && isRunning)
            {
                DrainStamina(10);
                HandleHungerAndThirst();
            }
        }

        private void HandleHungerAndThirst()
        {
            if (_hungerAttribute != null && _thirstAttribute != null)
            {
                _hungerAttribute.Value = Mathf.Max(0, _hungerAttribute.Value - 0.5f * Time.deltaTime);
                _thirstAttribute.Value = Mathf.Max(0, _thirstAttribute.Value - 0.5f * Time.deltaTime);
            }
        }

        private void DrainStamina(float ratePerSecond)
        {
            float drainAmount = ratePerSecond * Time.deltaTime;
            _staminaAttribute.Value = Mathf.Max(0, _staminaAttribute.Value - drainAmount);
        }

        private void SyncStaminaToMalbers()
        {
            // Enkelsynkning: Opsive styr, Malbers följer
            if (_staminaAttribute != null && _malbersStamina != null)
            {
                _malbersStamina.Value = _staminaAttribute.Value;
            }
        }

        // ➕ Vill du göra tvåvägssynk (ex. AI påverkar stamina) så kan du:
        private void SyncStaminaBidirectional()
        {
            float opsive = _staminaAttribute.Value;
            float malbers = _malbersStamina.Value;

            float average = (opsive + malbers) / 2f;

            _staminaAttribute.Value = average;
            _malbersStamina.Value = average;
        }
    }
}
