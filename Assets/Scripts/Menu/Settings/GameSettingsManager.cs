using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FantasyKingdom
{
    public class GameSettingsManager : MonoBehaviour
    {
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

        [Header("Gameplay Settings")]
        [SerializeField] private Toggle ShowTutorials;
        [SerializeField] private Slider ControllVibration;

        [Header("Save Options")]
        [SerializeField] private Toggle AutoSave;

        [Header("Difficulty")]
        [SerializeField] private Button[] AimAssist;
        [SerializeField] private TMP_Text AimAssistText;
        [SerializeField] private Toggle WallHackz;

        [Header("Graphics")]
        [SerializeField] private Button[] GraphicsMode;
        [SerializeField] private TMP_Text GraphicsQuality;

        [Header("Graphics Settings")]
        [SerializeField] private Slider ContrastSlider;
        [SerializeField] private Button[] AmbientOcclusion;
        [SerializeField] private TMP_Text AmbientOcclusionText;
        [SerializeField] private Button[] TextureQuality;
        [SerializeField] private TMP_Text TextureQualityText;
        [SerializeField] private Button[] TextureFiltering;
        [SerializeField] private TMP_Text TextureFilteringText;
        [SerializeField] private Button[] WaterQuality;
        [SerializeField] private TMP_Text WaterQualityText;
        [SerializeField] private Toggle BackgroundCryptoMining;
        [SerializeField] private Toggle RayTracedReflections;
        [SerializeField] private Toggle MotionBlur;
        [SerializeField] private Toggle DepthOfField;
        [SerializeField] private Toggle Early2000GameWeeFilter;
        [SerializeField] private Toggle Bloom;

        [Header("Language Settings")]
        [SerializeField] private Button[] AudioLanguage;
        [SerializeField] private TMP_Text AudioLanguageText;
        [SerializeField] private Button[] SubtitleLanguage;
        [SerializeField] private TMP_Text SubtitleLanguageText;
        [SerializeField] private Button[] GameplayerLangueage;
        [SerializeField] private TMP_Text GameplayerLangueageText;


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
            ContrastSlider.onValueChanged.AddListener(value =>
            {
                gameSettings.contrast = value;
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

            // Show Tutorials
            ShowTutorials.onValueChanged.AddListener(value =>
            {
                gameSettings.showTutorial = value;
                SaveSettings();
            });

            // Controller Vibration
            ControllVibration.onValueChanged.AddListener(value =>
            {
                gameSettings.controllervibration = value;
                SaveSettings();
            });

            // Auto Save
            AutoSave.onValueChanged.AddListener(value =>
            {
                gameSettings.autoSave = value;
                SaveSettings();
            });

            // Aim Assist
            foreach (var btn in AimAssist)
                btn.onClick.AddListener(() => SetAimAssist(btn));

            // Wall Hackz
            WallHackz.onValueChanged.AddListener(value =>
            {
                gameSettings.wallhackz = value;
                SaveSettings();
            });

            // Graphics Mode
            foreach (var btn in GraphicsMode)
                btn.onClick.AddListener(() => SetGraphicsMode(btn));

            // Ambient Occlusion
            foreach (var btn in AmbientOcclusion)
                btn.onClick.AddListener(() => SetAmbientOcclusion(btn));

            // Texture Quality
            foreach (var btn in TextureQuality)
                btn.onClick.AddListener(() => SetTextureQuality(btn));

            // Texture Filtering
            foreach (var btn in TextureFiltering)
                btn.onClick.AddListener(() => SetTextureFiltering(btn));

            // Water Quality
            foreach (var btn in WaterQuality)
                btn.onClick.AddListener(() => SetWaterQuality(btn));

            // Background Crypto Mining
            BackgroundCryptoMining.onValueChanged.AddListener(value =>
            {
                gameSettings.backgroundcryptomining = value;
                SaveSettings();
            });

            // Ray Traced Reflections
            RayTracedReflections.onValueChanged.AddListener(value =>
            {
                gameSettings.raytracedreflections = value;
                SaveSettings();
            });

            // Motion Blur
            MotionBlur.onValueChanged.AddListener(value =>
            {
                gameSettings.motionblur = value;
                SaveSettings();
            });

            // Depth Of Field
            DepthOfField.onValueChanged.AddListener(value =>
            {
                gameSettings.depthoffield = value;
                SaveSettings();
            });

            // Early 2000 Game Wee Filter
            Early2000GameWeeFilter.onValueChanged.AddListener(value =>
            {
                gameSettings.early2000gameweefilter = value;
                SaveSettings();
            });

            // Bloom
            Bloom.onValueChanged.AddListener(value =>
            {
                gameSettings.bloom = value;
                SaveSettings();
            });

            // Audio Language
            foreach (var btn in AudioLanguage)
                btn.onClick.AddListener(() => SetAudioLanguage(btn));

            // Subtitle Language
            foreach (var btn in SubtitleLanguage)
                btn.onClick.AddListener(() => SetSubtitleLanguage(btn));

            // Gameplay Language
            foreach (var btn in GameplayerLangueage)
                btn.onClick.AddListener(() => SetGameplayLanguage(btn));
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

        void SetAimAssist(Button selectedBtn)
        {
            gameSettings.aimassist = AimAssistText.text;
            AimAssistText.text = gameSettings.aimassist;
            SaveSettings();
        }

        void SetGraphicsMode(Button selectedBtn)
        {
            gameSettings.graphicsQuality = GraphicsQuality.text;
            GraphicsQuality.text = gameSettings.graphicsQuality;
            SaveSettings();
        }

        void SetAmbientOcclusion(Button selectedBtn)
        {
            gameSettings.ambientOcclusion = AmbientOcclusionText.text;
            AmbientOcclusionText.text = gameSettings.ambientOcclusion;
            SaveSettings();
        }

        void SetTextureQuality(Button selectedBtn)
        {
            gameSettings.textureQuality = TextureQualityText.text;
            TextureQualityText.text = gameSettings.textureQuality;
            SaveSettings();
        }

        void SetTextureFiltering(Button selectedBtn)
        {
            gameSettings.textureFiltering = TextureFilteringText.text;
            TextureFilteringText.text = gameSettings.textureFiltering;
            SaveSettings();
        }

        void SetWaterQuality(Button selectedBtn)
        {
            gameSettings.waterQuality = WaterQualityText.text;
            WaterQualityText.text = gameSettings.waterQuality;
            SaveSettings();
        }

        void SetAudioLanguage(Button selectedBtn)
        {
            gameSettings.audiolanguage = AudioLanguageText.text;
            AudioLanguageText.text = gameSettings.audiolanguage;
            SaveSettings();
        }

        void SetSubtitleLanguage(Button selectedBtn)
        {
            gameSettings.subtitleslanguage = SubtitleLanguageText.text;
            SubtitleLanguageText.text = gameSettings.subtitleslanguage;
            SaveSettings();
        }

        void SetGameplayLanguage(Button selectedBtn)
        {
            gameSettings.gameplaylanguage = GameplayerLangueageText.text;
            GameplayerLangueageText.text = gameSettings.gameplaylanguage;
            SaveSettings();
        }

        void ApplySettingsToUI()
        {
            DynamicRange.text = gameSettings.dynamicRange;
            MusicFrequency.text = gameSettings.musicFrequency;

            CinematicSubtitle.isOn = gameSettings.cinematicSubtitles;
            GamePlaySubtitle.isOn = gameSettings.gameplaySubtitles;
            TextSize.value = gameSettings.textSize;
            Opacity.value = gameSettings.opacity;

            ShowTutorials.isOn = gameSettings.showTutorial;
            ControllVibration.value = gameSettings.controllervibration;

            AutoSave.isOn = gameSettings.autoSave;

            AimAssistText.text = gameSettings.aimassist;
            WallHackz.isOn = gameSettings.wallhackz;

            GraphicsQuality.text = gameSettings.graphicsQuality;

            ContrastSlider.value = gameSettings.contrast;
            AmbientOcclusionText.text = gameSettings.ambientOcclusion;
            TextureQualityText.text = gameSettings.textureQuality;
            TextureFilteringText.text = gameSettings.textureFiltering;
            WaterQualityText.text = gameSettings.waterQuality;
            BackgroundCryptoMining.isOn = gameSettings.backgroundcryptomining;
            RayTracedReflections.isOn = gameSettings.raytracedreflections;
            MotionBlur.isOn = gameSettings.motionblur;
            DepthOfField.isOn = gameSettings.depthoffield;
            Early2000GameWeeFilter.isOn = gameSettings.early2000gameweefilter;
            Bloom.isOn = gameSettings.bloom;

            AudioLanguageText.text = gameSettings.audiolanguage;
            SubtitleLanguageText.text = gameSettings.subtitleslanguage;
            GameplayerLangueageText.text = gameSettings.gameplaylanguage;
        }

        void SaveSettings()
        {
            ES3.Save(settingsKey, gameSettings);
        }
    }
}
