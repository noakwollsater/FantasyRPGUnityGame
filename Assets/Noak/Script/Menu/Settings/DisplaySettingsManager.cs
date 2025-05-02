using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using Synty.Interface.FantasyMenus.Samples;
using ColorAdjustments = UnityEngine.Rendering.HighDefinition.ColorAdjustments;

namespace Unity.FantasyKingdom
{
    public class DisplaySettingsManager : MonoBehaviour
    {
        private const string settingsKey = "GameSettings";
        private GameSettingsData gameSettings;

        [Header("Settings Array")]
        [SerializeField] private SampleSettingsArray graphicModeSelector;
        [SerializeField] private SampleSettingsArray displayModeSelector;
        [SerializeField] private SampleSettingsArray resolutionSelector;
        [SerializeField] private SampleSettingsArray refreshRateSelector;

        [Header("Graphic Mode Settings")]
        [SerializeField] private Button graphicModeLeftButton;
        [SerializeField] private Button graphicModeRightButton;
        [SerializeField] private TMP_Text graphicModeText;

        [Header("Display Mode Settings")]
        [SerializeField] private Button displayModeLeftButton;
        [SerializeField] private Button displayModeRightButton;
        [SerializeField] private TMP_Text displayModeText;

        [Header("Resolution Settings")]
        [SerializeField] private Button resolutionLeftButton;
        [SerializeField] private Button resolutionRightButton;
        [SerializeField] private TMP_Text resolutionText;

        [Header("Refresh Rate Settings")]
        [SerializeField] private Button refreshRateLeftButton;
        [SerializeField] private Button refreshRateRightButton;
        [SerializeField] private TMP_Text refreshRateText;

        [Header("Post Processing")]
        [SerializeField] private Volume postProcessingVolume;
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private Slider initialBrightnessSlider;

        [SerializeField] private Toggle vSyncToggle;

        private ColorAdjustments colorAdjustments;

        private readonly string[] graphicModes = { "Low", "Medium", "High" };
        private readonly string[] displayModes = { "Fullscreen", "Windowed", "Borderless" };
        private readonly string[] resolutions = {
            "640x480", "800x600", "1024x768", "1280x720", "1280x800", "1360x768", "1366x768",
            "1440x900", "1600x900", "1680x1050", "1920x1080", "1920x1200", "2560x1080",
            "2560x1440", "3440x1440", "3840x2160", "5120x1440", "5120x2160", "7680x4320"
        };

        private readonly string[] refreshRates = { "60Hz", "75Hz", "120Hz", "144Hz", "165Hz", "240Hz" };

        private int currentGraphicModeIndex = 0;
        private int currentDisplayModeIndex = 0;
        private int currentResolutionIndex = 0;
        private int currentRefreshRateIndex = 0;

        void Start()
        {
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();

            currentGraphicModeIndex = System.Array.IndexOf(graphicModes, gameSettings.graphicsQuality);
            if (currentGraphicModeIndex < 0) currentGraphicModeIndex = 0;

            currentDisplayModeIndex = System.Array.IndexOf(displayModes, gameSettings.displaymode);
            if (currentDisplayModeIndex < 0) currentDisplayModeIndex = 0;

            currentResolutionIndex = System.Array.IndexOf(resolutions, gameSettings.resolution);
            if (currentResolutionIndex < 0) currentResolutionIndex = 0;

            currentRefreshRateIndex = System.Array.IndexOf(refreshRates, gameSettings.refreshRate);
            if (currentRefreshRateIndex < 0) currentRefreshRateIndex = 0;

            SetupUI();
            ApplyDisplaySettings();
            ApplyGraphicSettings();
        }

        private void SetUIValues()
        {
            if (refreshRateSelector != null)
            {
                refreshRateSelector.options = refreshRates;
                refreshRateSelector.defaultOption = currentRefreshRateIndex;
                refreshRateSelector.SetDefaultOption();
            }
            if (graphicModeSelector != null)
            {
                graphicModeSelector.options = graphicModes;
                graphicModeSelector.defaultOption = currentGraphicModeIndex;
                graphicModeSelector.SetDefaultOption();
            }
            if (displayModeSelector != null)
            {
                displayModeSelector.options = displayModes;
                displayModeSelector.defaultOption = currentDisplayModeIndex;
                displayModeSelector.SetDefaultOption();
            }
            if (resolutionSelector != null)
            {
                resolutionSelector.options = resolutions;
                resolutionSelector.defaultOption = currentResolutionIndex;
                resolutionSelector.SetDefaultOption();
            }
        }

        private void SetupUI()
        {
            SetUIValues();

            graphicModeLeftButton.onClick.AddListener(() =>
            {
                currentGraphicModeIndex = (currentGraphicModeIndex - 1 + graphicModes.Length) % graphicModes.Length;
                UpdateGraphicMode();
            });

            graphicModeRightButton.onClick.AddListener(() =>
            {
                currentGraphicModeIndex = (currentGraphicModeIndex + 1) % graphicModes.Length;
                UpdateGraphicMode();
            });

            displayModeLeftButton.onClick.AddListener(() =>
            {
                currentDisplayModeIndex = (currentDisplayModeIndex - 1 + displayModes.Length) % displayModes.Length;
                UpdateDisplayMode();
            });

            displayModeRightButton.onClick.AddListener(() =>
            {
                currentDisplayModeIndex = (currentDisplayModeIndex + 1) % displayModes.Length;
                UpdateDisplayMode();
            });

            resolutionLeftButton.onClick.AddListener(() =>
            {
                currentResolutionIndex = (currentResolutionIndex - 1 + resolutions.Length) % resolutions.Length;
                UpdateResolution();
            });

            resolutionRightButton.onClick.AddListener(() =>
            {
                currentResolutionIndex = (currentResolutionIndex + 1) % resolutions.Length;
                UpdateResolution();
            });

            refreshRateLeftButton.onClick.AddListener(() =>
            {
                currentRefreshRateIndex = (currentRefreshRateIndex - 1 + refreshRates.Length) % refreshRates.Length;
                UpdateRefreshRate();
            });

            refreshRateRightButton.onClick.AddListener(() =>
            {
                currentRefreshRateIndex = (currentRefreshRateIndex + 1) % refreshRates.Length;
                UpdateRefreshRate();
            });

            if (postProcessingVolume != null && postProcessingVolume.profile != null)
            {
                if (postProcessingVolume.profile.TryGet<ColorAdjustments>(out var ca))
                {
                    colorAdjustments = ca;
                    // Optionally set the slider's value to match saved brightness
                    float normalized = Mathf.InverseLerp(-5f, 5f, gameSettings.screenBrightness);
                    brightnessSlider.value = normalized * 100f;
                    colorAdjustments.postExposure.value = gameSettings.screenBrightness;
                }
                else
                {
                    Debug.LogWarning("ColorAdjustments not found in Post Processing Volume profile.");
                }
            }


            if (vSyncToggle != null)
            {
                vSyncToggle.isOn = gameSettings.vsync;
                vSyncToggle.onValueChanged.AddListener(value =>
                {
                    gameSettings.vsync = value;
                    ES3.Save(settingsKey, gameSettings);
                    ApplyVSync();
                });
            }

            graphicModeText.text = graphicModes[currentGraphicModeIndex];
            displayModeText.text = displayModes[currentDisplayModeIndex];
            resolutionText.text = resolutions[currentResolutionIndex];
            refreshRateText.text = refreshRates[currentRefreshRateIndex];
        }

        private void UpdateGraphicMode()
        {
            gameSettings.graphicsQuality = graphicModes[currentGraphicModeIndex];
            graphicModeText.text = gameSettings.graphicsQuality;
            ES3.Save(settingsKey, gameSettings);
            ApplyGraphicSettings();
        }

        private void ApplyGraphicSettings()
        {
            switch (gameSettings.graphicsQuality)
            {
                case "Low":
                    QualitySettings.SetQualityLevel(0);
                    break;
                case "Medium":
                    QualitySettings.SetQualityLevel(1);
                    break;
                case "High":
                    QualitySettings.SetQualityLevel(2);
                    break;
            }
        }

        private void UpdateDisplayMode()
        {
            gameSettings.displaymode = displayModes[currentDisplayModeIndex];
            displayModeText.text = gameSettings.displaymode;
            ES3.Save(settingsKey, gameSettings);
            ApplyDisplaySettings();
        }

        private void UpdateResolution()
        {
            gameSettings.resolution = resolutions[currentResolutionIndex];
            resolutionText.text = gameSettings.resolution;
            ES3.Save(settingsKey, gameSettings);
            ApplyDisplaySettings();
        }

        private void UpdateRefreshRate()
        {
            gameSettings.refreshRate = refreshRates[currentRefreshRateIndex];
            refreshRateText.text = gameSettings.refreshRate;
            ES3.Save(settingsKey, gameSettings);
            ApplyDisplaySettings();
        }

        private void ApplyDisplaySettings()
        {
            ApplyResolution();
            ApplyVSync();
        }

        private void ApplyResolution()
        {
            string[] parts = gameSettings.resolution.ToLower().Replace(" ", "").Split('x');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int width) || !int.TryParse(parts[1], out int height))
            {
                Debug.LogWarning($"Invalid resolution format: {gameSettings.resolution}");
                return;
            }

            int refreshRate = GetSelectedRefreshRate();
            FullScreenMode mode = GetSelectedFullScreenMode();

            Screen.SetResolution(width, height, mode, refreshRate);
            Debug.Log($"[ApplyResolution] {width}x{height} @ {refreshRate}Hz | Mode: {mode}");
        }

        private FullScreenMode GetSelectedFullScreenMode()
        {
            switch (gameSettings.displaymode.ToLower())
            {
                case "fullscreen": return FullScreenMode.ExclusiveFullScreen;
                case "borderless": return FullScreenMode.FullScreenWindow;
                case "windowed":
                default: return FullScreenMode.Windowed;
            }
        }

        private int GetSelectedRefreshRate()
        {
            if (!string.IsNullOrEmpty(gameSettings.refreshRate))
            {
                string hz = gameSettings.refreshRate.Replace("Hz", "").Trim();
                if (int.TryParse(hz, out int result))
                    return result;
            }
            return 60;
        }

        private void ApplyVSync()
        {
            QualitySettings.vSyncCount = gameSettings.vsync ? 1 : 0;
            Debug.Log($"[ApplyVSync] VSync: {(gameSettings.vsync ? "On" : "Off")}");
        }
        public void SetBrightness(float uiValue)
        {
            // Step 1: Validate the Volume
            if (postProcessingVolume == null)
            {
                Debug.LogWarning("PostProcessingVolume is not assigned.");
                return;
            }

            // Step 2: Validate the Volume Profile
            if (postProcessingVolume.profile == null)
            {
                Debug.LogWarning("PostProcessingVolume.profile is null.");
                return;
            }

            // Step 3: Try to get ColorAdjustments if not cached
            if (colorAdjustments == null)
            {
                if (!postProcessingVolume.profile.TryGet(out colorAdjustments))
                {
                    Debug.LogWarning("ColorAdjustments not found in the Volume Profile.");
                    return;
                }
            }

            // Step 4: Safely set exposure
            float mappedValue = Mathf.Lerp(-5f, 5f, uiValue / 100f);
            colorAdjustments.postExposure.value = mappedValue;
            gameSettings.screenBrightness = mappedValue;
            ES3.Save(settingsKey, gameSettings);

            OnBrightnessSliderChanged(uiValue); // <-- detta uppdaterar direkt

            Debug.Log($"[Brightness] UI Value: {uiValue} → Exposure: {mappedValue}");
        }
        // In DisplaySettingsManager.cs
        public void OnBrightnessSliderChanged(float uiValue)
        {
            DisplayManager.Instance?.SetBrightness(uiValue); // <-- detta uppdaterar direkt
        }

    }
}
