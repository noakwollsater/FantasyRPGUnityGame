using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class GameSettingsManager : MonoBehaviour
    {
        [Header("Brightness")]
        [SerializeField] private Slider brightnessSlider;

        [Header("Volume")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider DialogueVolumeSlider;

        [Header("Misc")]
        [SerializeField] private Button[] DynamicRangeBtns;
        [SerializeField] private Button[] MusicFrequencyBtns;
        [SerializeField] private TMP_Text DynamicRange;
        [SerializeField] private TMP_Text MusicFrequency;

        [Header("Subtitles")]
        [SerializeField] private Toggle CinematicSubtitle;
        [SerializeField] private Toggle GamePlaySubtitle;
        [SerializeField] private Slider TextSize;
        [SerializeField] private Slider Opacity;

        private GameSettingsData gameSettings;
        private const string settingsKey = "GameSettings";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();

            ApplySettingsToUI();
        }

        void OnEnable()
        {
            // Brightness
            brightnessSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.screenBrightness = value;
                SaveSettings();
            });

            //Master Volume
            masterVolumeSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.masterVolume = value;
                SaveSettings();
            });

            // SFX Volume
            sfxVolumeSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.sfxVolume = value;
                SaveSettings();
            });

            // Music Volume
            musicVolumeSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.musicVolume = value;
                SaveSettings();
            });

            // Dialogue Volume
            DialogueVolumeSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.dialogueVolume = value;
                SaveSettings();
            });

            // Dynamic Range
            foreach (var btn in DynamicRangeBtns)
                btn.onClick.AddListener(() => SetDynamicRange(btn));

            // Music Frequency
            foreach (var btn in MusicFrequencyBtns)
                btn.onClick.AddListener(() => SetMusicFrequency(btn));

            // Chinematic Subtitles
            CinematicSubtitle.onValueChanged.AddListener(value =>
            {
                gameSettings.cinematicSubtitles = value;
                SaveSettings();
            });

            // Gameplay Subtitles
            GamePlaySubtitle.onValueChanged.AddListener(value =>
            {
                gameSettings.gameplaySubtitles = value;
                SaveSettings();
            });

            // Text Size
            TextSize.onValueChanged.AddListener(value =>
            {
                gameSettings.textSize = value;
                SaveSettings();
            });

            // Opacity
            Opacity.onValueChanged.AddListener(value =>
            {
                gameSettings.opacity = value;
                SaveSettings();
            });
        }

        void SetDynamicRange(Button selectedBtn)
        {
            gameSettings.dynamicRange = DynamicRange.text;
            DynamicRange.text = gameSettings.dynamicRange;
            SaveSettings();
        }

        void SetMusicFrequency(Button selectedBtn)
        {
            gameSettings.musicFrequency = MusicFrequency.text;
            MusicFrequency.text = gameSettings.musicFrequency;
            SaveSettings();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void ApplySettingsToUI()
        {
            brightnessSlider.value = gameSettings.screenBrightness;

            masterVolumeSlider.value = gameSettings.masterVolume;
            sfxVolumeSlider.value = gameSettings.sfxVolume;
            musicVolumeSlider.value = gameSettings.musicVolume;
            DialogueVolumeSlider.value = gameSettings.dialogueVolume;

            DynamicRange.text = gameSettings.dynamicRange;
            MusicFrequency.text = gameSettings.musicFrequency;

            CinematicSubtitle.isOn = gameSettings.cinematicSubtitles;
            GamePlaySubtitle.isOn = gameSettings.gameplaySubtitles;
            TextSize.value = gameSettings.textSize;
            Opacity.value = gameSettings.opacity;
        }


        void SaveSettings()
        {
            ES3.Save(settingsKey, gameSettings);
        }
    }
}
