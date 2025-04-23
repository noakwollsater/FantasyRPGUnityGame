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

        [SerializeField] LoadCharacterData _loadCharacterData;

        [SerializeField] private Image _healthFillBar;
        [SerializeField] private Image _staminaFillBar;
        [SerializeField] private Image _hungerFillBar;
        [SerializeField] private Image _thirstFillBar;
        [SerializeField] private Image _manaFillBar;

        //When the bar is low, it will be red
        [SerializeField] private Image _healthLowFillBar;
        [SerializeField] private Image _staminaLowFillBar;
        [SerializeField] private Image _hungerLowFillBar;
        [SerializeField] private Image _thirstLowFillBar;
        [SerializeField] private Image _manaLowFillBar;

        void Start()
        {
            if (_loadCharacterData == null)
            {
                Debug.LogError("LoadCharacterData is not assigned in HotbarStats.");
                return;
            }
            _healthFillBar.fillAmount = 0.5f;
            _healthLowFillBar.fillAmount = 0.5f;

            Initstats();
        }

        private void Initstats()
        {
            float currentHP = _loadCharacterData.currentStats.HP;
            float currentStamina = _loadCharacterData.currentStats.Stamina;
            float currentHunger = _loadCharacterData.currentStats.Hunger;
            float currentThirst = _loadCharacterData.currentStats.Thirst;
            float currentMana = _loadCharacterData.currentStats.Mana;

            float maxHP = _loadCharacterData.finalStats.HP;
            float maxStamina = _loadCharacterData.finalStats.Stamina;
            float maxHunger = _loadCharacterData.finalStats.Hunger;
            float maxThirst = _loadCharacterData.finalStats.Thirst;
            float maxMana = _loadCharacterData.finalStats.Mana;

            _healthFillBar.fillAmount = Mathf.Clamp01(currentHP / maxHP);
            _staminaFillBar.fillAmount = Mathf.Clamp01(currentStamina / maxStamina);
            _hungerFillBar.fillAmount = Mathf.Clamp01(currentHunger / maxHunger);
            _thirstFillBar.fillAmount = Mathf.Clamp01(currentThirst / maxThirst);
            _manaFillBar.fillAmount = Mathf.Clamp01(currentMana / maxMana);

            _healthLowFillBar.fillAmount = Mathf.Clamp01(currentHP / maxHP);
            _staminaLowFillBar.fillAmount = Mathf.Clamp01(currentStamina / maxStamina);
            _hungerLowFillBar.fillAmount = Mathf.Clamp01(currentHunger / maxHunger);
            _thirstLowFillBar.fillAmount = Mathf.Clamp01(currentThirst / maxThirst);
            _manaLowFillBar.fillAmount = Mathf.Clamp01(currentMana / maxMana);
        }

        private void ModifyStat(StatType type, int amount)
        {
            Debug.Log($"Modifying {type} by {amount}");
            switch (type)
            {
                case StatType.HP:
                    _loadCharacterData.currentStats.HP = Mathf.Clamp(_loadCharacterData.currentStats.HP + amount, 0, _loadCharacterData.finalStats.HP);
                    UpdateBar(_healthFillBar, _healthLowFillBar, _loadCharacterData.currentStats.HP, _loadCharacterData.finalStats.HP);
                    break;
                case StatType.Stamina:
                    _loadCharacterData.currentStats.Stamina = Mathf.Clamp(_loadCharacterData.currentStats.Stamina + amount, 0, _loadCharacterData.finalStats.Stamina);
                    UpdateBar(_staminaFillBar, _staminaLowFillBar, _loadCharacterData.currentStats.Stamina, _loadCharacterData.finalStats.Stamina);
                    break;
                case StatType.Hunger:
                    _loadCharacterData.currentStats.Hunger = Mathf.Clamp(_loadCharacterData.currentStats.Hunger + amount, 0, _loadCharacterData.finalStats.Hunger);
                    UpdateBar(_hungerFillBar, _hungerLowFillBar, _loadCharacterData.currentStats.Hunger, _loadCharacterData.finalStats.Hunger);
                    break;
                case StatType.Thirst:
                    _loadCharacterData.currentStats.Thirst = Mathf.Clamp(_loadCharacterData.currentStats.Thirst + amount, 0, _loadCharacterData.finalStats.Thirst);
                    UpdateBar(_thirstFillBar, _thirstLowFillBar, _loadCharacterData.currentStats.Thirst, _loadCharacterData.finalStats.Thirst);
                    break;
                case StatType.Mana:
                    _loadCharacterData.currentStats.Mana = Mathf.Clamp(_loadCharacterData.currentStats.Mana + amount, 0, _loadCharacterData.finalStats.Mana);
                    UpdateBar(_manaFillBar, _manaLowFillBar, _loadCharacterData.currentStats.Mana, _loadCharacterData.finalStats.Mana);
                    break;
            }
        }
        private void UpdateBar(Image mainFill, Image lowFill, float current, float max)
        {
            float normalized = Mathf.Clamp01(current / max);
            mainFill.fillAmount = normalized;
            lowFill.fillAmount = normalized;
            Debug.Log($"Updated bar: {current}/{max} = {normalized}");
        }

        // Update is called once per frame
        void Update()
        {
            //Get key input to modify stats
            if (Input.GetKeyDown(KeyCode.B))
            {
                ModifyStat(StatType.HP, -10);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                ModifyStat(StatType.Stamina, -10);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ModifyStat(StatType.Hunger, -10);
            }
            if (Input.GetKeyDown(KeyCode.Comma))
            {
                ModifyStat(StatType.Thirst, -10);
            }
            if (Input.GetKeyDown(KeyCode.Period))
            {
                ModifyStat(StatType.Mana, -10);
            }
        }
    }
}
