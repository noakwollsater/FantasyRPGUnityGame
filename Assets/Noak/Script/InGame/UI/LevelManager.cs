using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Character Data")]
        [SerializeField] private LoadCharacterData _loadCharacterData;

        [Header("UI References")]
        [SerializeField] private Animator _levelAnimator;
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private GameObject levelPopUp;
        [SerializeField] private TMP_Text popUpLevelText;

        [Header("Animation Timing")]
        [SerializeField] private float levelPopUpDuration = 2f;

        private static readonly int LevelUpTrigger = Animator.StringToHash("LevelUp");

        private int _lastLevel = -1;
        private float _lastXP = -1;

        void Start()
        {
            if (_loadCharacterData == null)
            {
                Debug.LogError("LoadCharacterData not assigned!");
                enabled = false;
                return;
            }
            experienceBar.value = 0.5f;

            UpdateAllUI(force: true);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                AddExperience(100); // Test XP gain
            }
        }

        public void AddExperience(float amount)
        {
            if (_loadCharacterData == null) return;

            _loadCharacterData.experience += amount;

            while (_loadCharacterData.experience >= _loadCharacterData.experienceToNextLevel)
            {
                _loadCharacterData.experience -= _loadCharacterData.experienceToNextLevel;
                _loadCharacterData.level++;

                UpdateXPRequirement();
                OnLevelUp();
            }

            // Now that level and XP requirements are up to date, update bar
            UpdateXPBar();
        }


        private void UpdateXPRequirement()
        {
            int currentLevel = _loadCharacterData.level;
            float baseXP = 100f;
            float curve = 1.5f;

            _loadCharacterData.experienceToNextLevel = Mathf.Round(baseXP * Mathf.Pow(currentLevel, curve));
            Debug.Log($"New XP requirement: {_loadCharacterData.experienceToNextLevel} for level {currentLevel}");
        }

        private void OnLevelUp()
        {
            _lastLevel = _loadCharacterData.level;
            levelText.text = _lastLevel.ToString();
            popUpLevelText.text = _lastLevel.ToString();

            if (_levelAnimator != null)
                _levelAnimator.SetTrigger(LevelUpTrigger);

            if (levelPopUp != null)
            {
                levelPopUp.SetActive(true);
                Invoke(nameof(HideLevelPopUp), levelPopUpDuration);
            }

            Debug.Log("Level up! New level: " + _lastLevel);
        }

        private void HideLevelPopUp()
        {
            if (levelPopUp != null)
                levelPopUp.SetActive(false);
        }

        private void UpdateXPBar()
        {
            float currentXP = _loadCharacterData.experience;
            float requiredXP = _loadCharacterData.experienceToNextLevel;

            float normalizedXP = requiredXP > 0 ? Mathf.Clamp01(currentXP / requiredXP) : 0f;

            _lastXP = currentXP;

            if (experienceBar != null)
                experienceBar.value = normalizedXP;

            Debug.Log($"XP updated: {currentXP}/{requiredXP} = {normalizedXP}");
        }

        private void UpdateAllUI(bool force = false)
        {
            if (force || _loadCharacterData.level != _lastLevel)
                levelText.text = _loadCharacterData.level.ToString();

            UpdateXPBar();
        }
    }
}
