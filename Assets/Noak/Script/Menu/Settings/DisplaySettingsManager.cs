using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unity.FantasyKingdom
{
    public class DisplaySettingsManager : MonoBehaviour
    {
        private const string settingsKey = "GameSettings";
        private GameSettingsData gameSettings;

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

        [SerializeField] private Toggle vSyncToggle;

        private readonly string[] displayModes = { "Fullscreen", "Windowed", "Borderless" };
        private readonly string[] resolutions = { "1920x1080", "1280x720", "2560x1440", "2560x1080", "3840x2160", "1024x768" };
        private readonly string[] refreshRates = { "60Hz", "75Hz", "120Hz", "144Hz", "165Hz", "240Hz" };

        private int currentDisplayModeIndex = 0;
        private int currentResolutionIndex = 0;
        private int currentRefreshRateIndex = 0;

        void Start()
        {
            // Load or create settings
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();

            // Initialize indexes
            currentDisplayModeIndex = System.Array.IndexOf(displayModes, gameSettings.displaymode);
            if (currentDisplayModeIndex < 0) currentDisplayModeIndex = 0;

            currentResolutionIndex = System.Array.IndexOf(resolutions, gameSettings.resolution);
            if (currentResolutionIndex < 0) currentResolutionIndex = 0;

            currentRefreshRateIndex = System.Array.IndexOf(refreshRates, gameSettings.refreshRate);
            if (currentRefreshRateIndex < 0) currentRefreshRateIndex = 0;

            SetupUI();
            ApplyDisplaySettings();
        }

        private void SetupUI()
        {
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

            // Set initial UI text
            displayModeText.text = displayModes[currentDisplayModeIndex];
            resolutionText.text = resolutions[currentResolutionIndex];
            refreshRateText.text = refreshRates[currentRefreshRateIndex];
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
            ApplyResolution(); // Handles mode + refresh too
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
    }
}
