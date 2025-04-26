using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Opsive.UltimateCharacterController.Traits;

namespace Unity.FantasyKingdom
{
    public class HotbarStats : MonoBehaviour
    {
        public enum StatType
        {
            Health,
            Stamina,
            Hunger,
            Thirst,
            Mana
        }

        [SerializeField] private LoadCharacterData _loadCharacterData;
        [SerializeField] private AttributeManager _attributeManager;
        [SerializeField] private Health _health;
        [SerializeField] private DeathUIManager _deathUIManager;

        private Attribute _healthAttribute;

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

        private void OnEnable()
        {
            if (_attributeManager != null)
            {
                UpdateAllBars();
            }
            else
            {
                Debug.Log("HotbarStats: Skipped UpdateAllBars in OnEnable because _attributeManager is null.");
            }
        }

        public void DamageHealthOverTime(float damagePerSecond)
        {
            if (_health == null || _healthAttribute == null)
            {
                Debug.LogWarning("HotbarStats: Health component or Attribute is null. Cannot damage health.");
                return;
            }

            float damage = damagePerSecond * Time.deltaTime;
            _healthAttribute.Value -= damage;

            if (_damageFX != null)
            {
                _damageFX.SetTrigger(DamageFXParam); // Play damage animation
            }

            if (_healthAttribute.Value <= 0)
            {
                _health.ImmediateDeath();
            }
        }


        public void setAttributeManager()
        {
            GameObject character = GameObject.FindGameObjectWithTag("Player");

            if (character == null)
            {
                Debug.LogError("HotbarStats: No GameObject with tag 'Player' found.");
                return;
            }

            _attributeManager = character.GetComponent<CharacterAttributeManager>();
            _healthAttribute = _attributeManager.GetAttribute("Health");

            if (_attributeManager == null)
            {
                Debug.LogError("HotbarStats: Character does not have a CharacterAttributeManager component.");
                return;
            }

            _health = character.GetComponent<Health>();

            Opsive.Shared.Events.EventHandler.RegisterEvent<Attribute>(_attributeManager.gameObject, "OnAttributeUpdateValue", OnAttributeUpdated);

            InitStats(); // Initialize stats after setting the attribute manager
        }

        public void Death()
        {
            GameObject character = GameObject.FindGameObjectWithTag("Player");
            Health health = character.GetComponent<Health>();
            if (health == null)
            {
                Debug.LogError("HotbarStats: No Health component found on the character.");
                return;
            }
            health.ImmediateDeath();
        }

        public void InitStats()
        {
            if (_attributeManager == null || _loadCharacterData == null)
            {
                Debug.LogError("HotbarStats: Missing _attributeManager or _loadCharacterData.");
                return;
            }

            SetAttributeValue(StatType.Stamina, _loadCharacterData.currentStats.Stamina, _loadCharacterData.finalStats.Stamina);
            SetAttributeValue(StatType.Hunger, _loadCharacterData.currentStats.Hunger, _loadCharacterData.finalStats.Hunger);
            SetAttributeValue(StatType.Thirst, _loadCharacterData.currentStats.Thirst, _loadCharacterData.finalStats.Thirst);
            SetAttributeValue(StatType.Mana, _loadCharacterData.currentStats.Mana, _loadCharacterData.finalStats.Mana);
            SetAttributeValue(StatType.Health, _loadCharacterData.currentStats.HP, _loadCharacterData.finalStats.HP);

            StartCoroutine(DelayedUpdateBars()); // 🠖 this is the fix
        }

        private IEnumerator DelayedUpdateBars()
        {
            yield return null; // wait one frame
            UpdateAllBars();
        }

        private void SetAttributeValue(StatType type, int value, int maxValue)
        {
            var attribute = _attributeManager.GetAttribute(GetAttributeName(type));
            if (attribute != null)
            {
                attribute.MaxValue = maxValue;
                attribute.Value = value;
            }
            else
            {
                Debug.LogWarning($"HotbarStats: Attribute {type} not found.");
            }
        }

        private void OnAttributeUpdated(Attribute attribute)
        {
            if (attribute.Name == "Health")
            {
                _loadCharacterData.currentStats.HP = Mathf.RoundToInt(attribute.Value);
                UpdateStatBar(StatType.Health, attribute.Value, attribute.MaxValue);
            }
            else if (attribute.Name == "Stamina")
            {
                _loadCharacterData.currentStats.Stamina = Mathf.RoundToInt(attribute.Value);
                UpdateStatBar(StatType.Stamina, attribute.Value, attribute.MaxValue);
            }

            else if (attribute.Name == "Hunger")
            {
                _loadCharacterData.currentStats.Hunger = Mathf.RoundToInt(attribute.Value);
                UpdateStatBar(StatType.Hunger, attribute.Value, attribute.MaxValue);
            }
            else if (attribute.Name == "Thirst")
            {
                _loadCharacterData.currentStats.Thirst = Mathf.RoundToInt(attribute.Value);
                UpdateStatBar(StatType.Thirst, attribute.Value, attribute.MaxValue);
            }
            else if (attribute.Name == "Mana")
            {
                _loadCharacterData.currentStats.Mana = Mathf.RoundToInt(attribute.Value);
                UpdateStatBar(StatType.Mana, attribute.Value, attribute.MaxValue);
            }
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

            if (amount < 0 && type == StatType.Health)
                _damageFX.SetTrigger(DamageFXParam);
            else if (amount > 0 && type == StatType.Health)
                _healFX.SetTrigger(HealFXParam);

            if (newValue <= 0 && type == StatType.Health)
            {
                _health.ImmediateDeath();
            }

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
                case StatType.Health:
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
        }

        private float GetCurrentStat(StatType type)
        {
            if (_attributeManager == null)
            {
                Debug.LogError("HotbarStats: _attributeManager is null.");
                return 0;
            }
            var attr = _attributeManager.GetAttribute(GetAttributeName(type));
            return attr != null ? attr.Value : 0;
        }

        private float GetMaxStat(StatType type)
        {
            if (_attributeManager == null)
            {
                Debug.LogError("HotbarStats: _attributeManager is null.");
                return 0;
            }
            var attr = _attributeManager.GetAttribute(GetAttributeName(type));
            return attr != null ? attr.MaxValue : 0;
        }

        private void SetCurrentStat(StatType type, float value)
        {
            var attr = _attributeManager.GetAttribute(GetAttributeName(type));
            if (attr != null)
            {
                attr.Value = Mathf.Clamp(value, attr.MinValue, attr.MaxValue);
            }
        }

        private string GetAttributeName(StatType type)
        {
            switch (type)
            {
                case StatType.Health: return "Health";
                case StatType.Stamina: return "Stamina";
                case StatType.Hunger: return "Hunger";
                case StatType.Thirst: return "Thirst";
                case StatType.Mana: return "Mana";
                default: return "";
            }
        }

        void Update()
        {
            // For testing: reduce stats with keys
            if (Input.GetKeyDown(KeyCode.B)) ModifyStat(StatType.Health, -10);
            if (Input.GetKeyDown(KeyCode.L)) ModifyStat(StatType.Health, +10);
            if (Input.GetKeyDown(KeyCode.N)) ModifyStat(StatType.Stamina, -10);
            if (Input.GetKeyDown(KeyCode.M)) ModifyStat(StatType.Hunger, +10);
            if (Input.GetKeyDown(KeyCode.Comma)) ModifyStat(StatType.Thirst, +10);
            if (Input.GetKeyDown(KeyCode.Period)) ModifyStat(StatType.Mana, -10);
        }
    }
}