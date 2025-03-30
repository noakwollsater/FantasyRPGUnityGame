using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class SoundSettingManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource dialogueSource;

        [Header("Volume Sliders (0 - 100)")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider dialogueVolumeSlider;

        private const string settingsKey = "GameSettings";
        private GameSettingsData gameSettings;

        void Start()
        {
            LoadSettings();
            ApplySoundSettings();
            SetupSliderListeners();
        }

        private void LoadSettings()
        {
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();
        }

        private void ApplySoundSettings()
        {
            // Convert internal volume (0-1) to UI slider (0-100)
            if (masterVolumeSlider != null) masterVolumeSlider.value = gameSettings.masterVolume * 100f;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = gameSettings.sfxVolume * 100f;
            if (musicVolumeSlider != null) musicVolumeSlider.value = gameSettings.musicVolume * 100f;
            if (dialogueVolumeSlider != null) dialogueVolumeSlider.value = gameSettings.dialogueVolume * 100f;

            // Apply volumes to AudioSources (0-1 scale)
            float master = Mathf.Clamp01(gameSettings.masterVolume);
            if (musicSource != null) musicSource.volume = Mathf.Clamp01(gameSettings.musicVolume) * master;
            if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(gameSettings.sfxVolume) * master;
            if (dialogueSource != null) dialogueSource.volume = Mathf.Clamp01(gameSettings.dialogueVolume) * master;
        }

        private void SetupSliderListeners()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(value =>
                {
                    gameSettings.masterVolume = value / 100f;
                    ApplySoundSettings();
                    SaveSettings();
                });

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(value =>
                {
                    gameSettings.sfxVolume = value / 100f;
                    ApplySoundSettings();
                    SaveSettings();
                });

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(value =>
                {
                    gameSettings.musicVolume = value / 100f;
                    ApplySoundSettings();
                    SaveSettings();
                });

            if (dialogueVolumeSlider != null)
                dialogueVolumeSlider.onValueChanged.AddListener(value =>
                {
                    gameSettings.dialogueVolume = value / 100f;
                    ApplySoundSettings();
                    SaveSettings();
                });
        }

        private void SaveSettings()
        {
            ES3.Save(settingsKey, gameSettings);
        }
    }
}
