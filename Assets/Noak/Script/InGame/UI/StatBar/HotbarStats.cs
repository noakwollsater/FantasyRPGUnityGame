using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        [SerializeField] private Animator _damageFX;
        [SerializeField] private Animator _healFX;
        [SerializeField] private Animator _poisenedFX;

        [SerializeField] private float lowStatThreshold = 0.2f;

        private static readonly int HealthParam = Animator.StringToHash("Health");
        private static readonly int StaminaParam = Animator.StringToHash("Health");
        private static readonly int HungerParam = Animator.StringToHash("Health");
        private static readonly int ThirstParam = Animator.StringToHash("Health");
        private static readonly int ManaParam = Animator.StringToHash("Health");

        private static readonly int DamageFXParam = Animator.StringToHash("Hit");
        private static readonly int HealFXParam = Animator.StringToHash("Heal");
        private static readonly int PoisenedFXParam = Animator.StringToHash("Poisoned");

        private Coroutine _statCoroutine;

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
            foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
            {
                UpdateStatBar(stat, GetCurrentStat(stat), GetMaxStat(stat));
            }
        }

        private void ModifyStat(StatType type, int amount)
        {
            float current = GetCurrentStat(type);
            float max = GetMaxStat(type);
            float newValue = Mathf.Clamp(current + amount, 0, max);

            if (amount < 0)
                _damageFX.SetTrigger(DamageFXParam);
            else if (amount > 0)
                _healFX.SetTrigger(HealFXParam);

            if (_statCoroutine != null)
                StopCoroutine(_statCoroutine);

            _statCoroutine = StartCoroutine(SmoothModifyStat(type, newValue));
        }

        private IEnumerator SmoothModifyStat(StatType type, float targetValue, float duration = 0.5f)
        {
            float currentValue = GetCurrentStat(type);
            float elapsed = 0f;
            float initialValue = currentValue;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float newValue = Mathf.Lerp(initialValue, targetValue, elapsed / duration);
                SetCurrentStat(type, newValue);
                UpdateStatBar(type, newValue, GetMaxStat(type));
                yield return null;
            }

            SetCurrentStat(type, targetValue);
            UpdateStatBar(type, targetValue, GetMaxStat(type));
        }

        private void UpdateStatBar(StatType type, float current, float max)
        {
            float normalized = Mathf.Clamp01(current / max);

            switch (type)
            {
                case StatType.HP:
                    _healthLowFillBar.fillAmount = normalized;
                    if (_healthAnimator != null)
                        _healthAnimator.SetFloat(HealthParam, normalized);
                    else
                        _healthFillBar.fillAmount = normalized;
                    break;

                case StatType.Stamina:
                    _staminaLowFillBar.fillAmount = normalized;
                    if (_staminaAnimator != null)
                        _staminaAnimator.SetFloat(StaminaParam, normalized);
                    else
                        _staminaFillBar.fillAmount = normalized;
                    break;

                case StatType.Hunger:
                    _hungerLowFillBar.fillAmount = normalized;
                    if (_hungerAnimator != null)
                        _hungerAnimator.SetFloat(HungerParam, normalized);
                    else
                        _hungerFillBar.fillAmount = normalized;
                    break;

                case StatType.Thirst:
                    _thirstLowFillBar.fillAmount = normalized;
                    if (_thirstAnimator != null)
                        _thirstAnimator.SetFloat(ThirstParam, normalized);
                    else
                        _thirstFillBar.fillAmount = normalized;
                    break;

                case StatType.Mana:
                    _manaLowFillBar.fillAmount = normalized;
                    if (_manaAnimator != null)
                        _manaAnimator.SetFloat(ManaParam, normalized);
                    else
                        _manaFillBar.fillAmount = normalized;
                    break;
            }

            Debug.Log($"{type} updated: {current}/{max} = {normalized}");
        }

        private float GetCurrentStat(StatType type)
        {
            switch (type)
            {
                case StatType.HP: return _loadCharacterData.currentStats.HP;
                case StatType.Stamina: return _loadCharacterData.currentStats.Stamina;
                case StatType.Hunger: return _loadCharacterData.currentStats.Hunger;
                case StatType.Thirst: return _loadCharacterData.currentStats.Thirst;
                case StatType.Mana: return _loadCharacterData.currentStats.Mana;
                default: return 0;
            }
        }

        private float GetMaxStat(StatType type)
        {
            switch (type)
            {
                case StatType.HP: return _loadCharacterData.finalStats.HP;
                case StatType.Stamina: return _loadCharacterData.finalStats.Stamina;
                case StatType.Hunger: return _loadCharacterData.finalStats.Hunger;
                case StatType.Thirst: return _loadCharacterData.finalStats.Thirst;
                case StatType.Mana: return _loadCharacterData.finalStats.Mana;
                default: return 0;
            }
        }

        private void SetCurrentStat(StatType type, float value)
        {
            int intValue = Mathf.RoundToInt(value); // Round to closest int

            switch (type)
            {
                case StatType.HP: _loadCharacterData.currentStats.HP = intValue; break;
                case StatType.Stamina: _loadCharacterData.currentStats.Stamina = intValue; break;
                case StatType.Hunger: _loadCharacterData.currentStats.Hunger = intValue; break;
                case StatType.Thirst: _loadCharacterData.currentStats.Thirst = intValue; break;
                case StatType.Mana: _loadCharacterData.currentStats.Mana = intValue; break;
            }
        }

        void Update()
        {
            // For testing: reduce stats with keys
            if (Input.GetKeyDown(KeyCode.B)) ModifyStat(StatType.HP, -10);
            if (Input.GetKeyDown(KeyCode.L)) ModifyStat(StatType.HP, +10);
            if (Input.GetKeyDown(KeyCode.N)) ModifyStat(StatType.Stamina, -10);
            if (Input.GetKeyDown(KeyCode.M)) ModifyStat(StatType.Hunger, -10);
            if (Input.GetKeyDown(KeyCode.Comma)) ModifyStat(StatType.Thirst, -10);
            if (Input.GetKeyDown(KeyCode.Period)) ModifyStat(StatType.Mana, -10);
        }
    }
}
