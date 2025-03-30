using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class DisplaySettingsManager : MonoBehaviour
    {
        private const string settingsKey = "GameSettings";
        private GameSettingsData gameSettings;

        void Start()
        {
            // Load saved settings
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData();

            ApplyDisplaySettings();
        }

        private void ApplyDisplaySettings()
        {
            // Set Display Mode
            switch (gameSettings.displaymode.ToLower())
            {
                case "Fullscreen":
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case "Windowed":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                case "Borderless":
                case "Fullscreen window":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
            }

            // Set Resolution
            string[] resolutionParts = gameSettings.resolution.ToLower().Replace(" ", "").Split('x');
            if (resolutionParts.Length == 2 &&
                int.TryParse(resolutionParts[0], out int width) &&
                int.TryParse(resolutionParts[1], out int height))
            {
                int refreshRate = 60; // Default

                if (!string.IsNullOrEmpty(gameSettings.refreshRate))
                {
                    string refreshString = gameSettings.refreshRate.Replace("Hz", "").Trim();
                    int.TryParse(refreshString, out refreshRate);
                }

                Screen.SetResolution(width, height, Screen.fullScreenMode, refreshRate);
            }

            // Set VSync
            QualitySettings.vSyncCount = gameSettings.vsync ? 1 : 0;
        }
    }
}
