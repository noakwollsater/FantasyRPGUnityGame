using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class HotbarStats : MonoBehaviour
    {
        public enum StatType
        {
            HP,
            Stamina,
            Hunger,
            Thirst,
            Mana
        }

        [SerializeField] private LoadCharacterData _loadCharacterData;

        [SerializeField] private Image _healthFillBar, _staminaFillBar, _hungerFillBar, _thirstFillBar, _manaFillBar;
        [SerializeField] private Image _healthLowFillBar, _staminaLowFillBar, _hungerLowFillBar, _thirstLowFillBar, _manaLowFillBar;

        [Header("Animation")]
        [SerializeField] private Animator _healthAnimator;
        [SerializeField] private Animator _staminaAnimator;
        [SerializeField] private Animator _hungerAnimator;
        [SerializeField] private Animator _thirstAnimator;
        [SerializeField] private Animator _manaAnimator;

        [SerializeField] private float lowStatThreshold = 0.2f;

        // Animator parameter hashes
        private static readonly int HealthParam = Animator.StringToHash("Health");
        private static readonly int IsLowHealth = Animator.StringToHash("IsLowHealth");

        private static readonly int StaminaParam = Animator.StringToHash("Health");
        private static readonly int IsLowStamina = Animator.StringToHash("IsLowStamina");

        private static readonly int HungerParam = Animator.StringToHash("Health");
        private static readonly int IsLowHunger = Animator.StringToHash("IsLowHunger");

        private static readonly int ThirstParam = Animator.StringToHash("Health");
        private static readonly int IsLowThirst = Animator.StringToHash("IsLowThirst");

        private static readonly int ManaParam = Animator.StringToHash("Health");
        private static readonly int IsLowMana = Animator.StringToHash("IsLowMana");

        void Start()
        {
            if (_loadCharacterData == null)
            {
                Debug.LogError("LoadCharacterData is not assigned in HotbarStats.");
                return;
            }

            InitStats();
        }

        private void InitStats()
        {
            UpdateAllBars();
        }

        private void UpdateAllBars()
        {
            UpdateStatBar(StatType.HP, _loadCharacterData.currentStats.HP, _loadCharacterData.finalStats.HP);
            UpdateStatBar(StatType.Stamina, _loadCharacterData.currentStats.Stamina, _loadCharacterData.finalStats.Stamina);
            UpdateStatBar(StatType.Hunger, _loadCharacterData.currentStats.Hunger, _loadCharacterData.finalStats.Hunger);
            UpdateStatBar(StatType.Thirst, _loadCharacterData.currentStats.Thirst, _loadCharacterData.finalStats.Thirst);
            UpdateStatBar(StatType.Mana, _loadCharacterData.currentStats.Mana, _loadCharacterData.finalStats.Mana);
        }

        private void ModifyStat(StatType type, int amount)
        {
            Debug.Log($"Modifying {type} by {amount}");

            switch (type)
            {
                case StatType.HP:
                    _loadCharacterData.currentStats.HP = Mathf.Clamp(_loadCharacterData.currentStats.HP + amount, 0, _loadCharacterData.finalStats.HP);
                    break;
                case StatType.Stamina:
                    _loadCharacterData.currentStats.Stamina = Mathf.Clamp(_loadCharacterData.currentStats.Stamina + amount, 0, _loadCharacterData.finalStats.Stamina);
                    break;
                case StatType.Hunger:
                    _loadCharacterData.currentStats.Hunger = Mathf.Clamp(_loadCharacterData.currentStats.Hunger + amount, 0, _loadCharacterData.finalStats.Hunger);
                    break;
                case StatType.Thirst:
                    _loadCharacterData.currentStats.Thirst = Mathf.Clamp(_loadCharacterData.currentStats.Thirst + amount, 0, _loadCharacterData.finalStats.Thirst);
                    break;
                case StatType.Mana:
                    _loadCharacterData.currentStats.Mana = Mathf.Clamp(_loadCharacterData.currentStats.Mana + amount, 0, _loadCharacterData.finalStats.Mana);
                    break;
            }

            UpdateAllBars();
        }

        private void UpdateStatBar(StatType type, float current, float max)
        {
            float normalized = Mathf.Clamp01(current / max);

            switch (type)
            {
                case StatType.HP:
                    _healthLowFillBar.fillAmount = normalized;
                    if (_healthAnimator != null)
                    {
                        _healthAnimator.SetFloat(HealthParam, normalized);
                        _healthAnimator.SetBool(IsLowHealth, normalized <= lowStatThreshold);
                    }
                    else _healthFillBar.fillAmount = normalized;
                    break;

                case StatType.Stamina:
                    _staminaLowFillBar.fillAmount = normalized;
                    if (_staminaAnimator != null)
                    {
                        _staminaAnimator.SetFloat(StaminaParam, normalized);
                        _staminaAnimator.SetBool(IsLowStamina, normalized <= lowStatThreshold);
                    }
                    else _staminaFillBar.fillAmount = normalized;
                    break;

                case StatType.Hunger:
                    _hungerLowFillBar.fillAmount = normalized;
                    if (_hungerAnimator != null)
                    {
                        _hungerAnimator.SetFloat(HungerParam, normalized);
                        _hungerAnimator.SetBool(IsLowHunger, normalized <= lowStatThreshold);
                    }
                    else _hungerFillBar.fillAmount = normalized;
                    break;

                case StatType.Thirst:
                    _thirstLowFillBar.fillAmount = normalized;
                    if (_thirstAnimator != null)
                    {
                        _thirstAnimator.SetFloat(ThirstParam, normalized);
                        _thirstAnimator.SetBool(IsLowThirst, normalized <= lowStatThreshold);
                    }
                    else _thirstFillBar.fillAmount = normalized;
                    break;

                case StatType.Mana:
                    _manaLowFillBar.fillAmount = normalized;
                    if (_manaAnimator != null)
                    {
                        _manaAnimator.SetFloat(ManaParam, normalized);
                        _manaAnimator.SetBool(IsLowMana, normalized <= lowStatThreshold);
                    }
                    else _manaFillBar.fillAmount = normalized;
                    break;
            }

            Debug.Log($"{type} updated: {current}/{max} = {normalized}");
        }

        void Update()
        {
            // For testing: reduce stats with keys
            if (Input.GetKeyDown(KeyCode.B)) ModifyStat(StatType.HP, -10);
            if (Input.GetKeyDown(KeyCode.N)) ModifyStat(StatType.Stamina, -10);
            if (Input.GetKeyDown(KeyCode.M)) ModifyStat(StatType.Hunger, -10);
            if (Input.GetKeyDown(KeyCode.Comma)) ModifyStat(StatType.Thirst, -10);
            if (Input.GetKeyDown(KeyCode.Period)) ModifyStat(StatType.Mana, -10);
        }
    }
}
