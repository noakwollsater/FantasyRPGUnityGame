using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Unity.FantasyKingdom
{
    public class DisplaySettingsManager : MonoBehaviour
    {
        private const string settingsKey = "GameSettings";
        private GameSettingsData gameSettings;

        [Header("Display Settings")]
        [SerializeField] private Button[] displayModeButtons;
        [SerializeField] private TMP_Text displayModeText;

        [SerializeField] private Button[] resolutionButtons;
        [SerializeField] private TMP_Text resolutionText;

        [SerializeField] private Button[] refreshRateButtons;
        [SerializeField] private TMP_Text refreshRateText;

        [SerializeField] private Toggle vSyncToggle;

        void Start()
        {
            // Load saved settings
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();

            ApplyDisplaySettings();
            SetupUI();
        }

        private void SetupUI()
        {
            // Display Mode Buttons
            foreach (var btn in displayModeButtons)
            {
                btn.onClick.AddListener(() =>
                {
                    gameSettings.displaymode = displayModeText.text;
                    ES3.Save(settingsKey, gameSettings);
                    ApplyDisplaySettings();
                });
            }

            // Resolution Buttons
            foreach (var btn in resolutionButtons)
            {
                btn.onClick.AddListener(() =>
                {
                    gameSettings.resolution = resolutionText.text;
                    ES3.Save(settingsKey, gameSettings);
                    ApplyDisplaySettings();
                });
            }

            // Refresh Rate Buttons
            foreach (var btn in refreshRateButtons)
            {
                btn.onClick.AddListener(() =>
                {
                    gameSettings.refreshRate = refreshRateText.text;
                    ES3.Save(settingsKey, gameSettings);
                    ApplyDisplaySettings();
                });
            }

            // VSync Toggle
            if (vSyncToggle != null)
            {
                vSyncToggle.isOn = gameSettings.vsync;
                vSyncToggle.onValueChanged.AddListener(value =>
                {
                    gameSettings.vsync = value;
                    ES3.Save(settingsKey, gameSettings);
                    ApplyDisplaySettings();
                });
            }

            // Update UI texts with saved data
            displayModeText.text = gameSettings.displaymode;
            resolutionText.text = gameSettings.resolution;
            refreshRateText.text = gameSettings.refreshRate;
        }

        private void ApplyDisplaySettings()
        {
            // Set Display Mode
            switch (gameSettings.displaymode.ToLower())
            {
                case "fullscreen":
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case "windowed":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case "borderless":
                case "fullscreen window":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }

            // Set Resolution
            string[] parts = gameSettings.resolution.ToLower().Replace(" ", "").Split('x');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int width) &&
                int.TryParse(parts[1], out int height))
            {
                int refreshRate = 60;
                if (!string.IsNullOrEmpty(gameSettings.refreshRate))
                {
                    string refresh = gameSettings.refreshRate.Replace("Hz", "").Trim();
                    int.TryParse(refresh, out refreshRate);
                }

                Screen.SetResolution(width, height, Screen.fullScreenMode, refreshRate);
            }
            else
            {
                Debug.LogWarning($"Invalid resolution format: {gameSettings.resolution}");
            }

            // VSync
            QualitySettings.vSyncCount = gameSettings.vsync ? 1 : 0;
        }
    }
}
