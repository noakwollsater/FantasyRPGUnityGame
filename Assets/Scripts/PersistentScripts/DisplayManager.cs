using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager Instance;

    private const string settingsKey = "GameSettings";
    private GameSettingsData gameSettings;

    [Header("Optional: Scene-wide Post-processing Volume")]
    [SerializeField] private Volume postProcessingVolume;

    private ColorAdjustments colorAdjustments;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LoadSettings()
    {
        if (gameSettings != null) return; // ✅ Laddades redan

        if (ES3.KeyExists(settingsKey))
            gameSettings = ES3.Load<GameSettingsData>(settingsKey);
        else
            gameSettings = new GameSettingsData();
    }


    private void ApplyDisplaySettings()
    {
        ApplyResolution();
        ApplyVSync();
        ApplyGraphicSettings();
        ApplyBrightness();
    }

    private void ApplyResolution()
    {
        string[] parts = gameSettings.resolution?.ToLower()?.Replace(" ", "")?.Split('x');
        if (parts == null || parts.Length != 2 || !int.TryParse(parts[0], out int width) || !int.TryParse(parts[1], out int height))
        {
            Debug.LogWarning($"Invalid resolution: {gameSettings.resolution}");
            return;
        }

        int refreshRate = GetSelectedRefreshRate();
        FullScreenMode mode = GetSelectedFullScreenMode();

        Screen.SetResolution(width, height, mode, refreshRate);
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

    private FullScreenMode GetSelectedFullScreenMode()
    {
        switch (gameSettings.displaymode?.ToLower())
        {
            case "fullscreen": return FullScreenMode.ExclusiveFullScreen;
            case "borderless": return FullScreenMode.FullScreenWindow;
            case "windowed": default: return FullScreenMode.Windowed;
        }
    }

    private void ApplyVSync()
    {
        QualitySettings.vSyncCount = gameSettings.vsync ? 1 : 0;
    }

    private void ApplyGraphicSettings()
    {
        switch (gameSettings.graphicsQuality)
        {
            case "High":
                SetQualityByName("High Fidelity");
                break;
            case "Medium":
                SetQualityByName("Balanced");
                break;
            case "Low":
                SetQualityByName("Performant");
                break;
            default:
                Debug.LogWarning($"Unknown graphics quality: {gameSettings.graphicsQuality}");
                break;
        }
    }
    private void SetQualityByName(string qualityName)
    {
        var names = QualitySettings.names;
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Equals(qualityName, System.StringComparison.OrdinalIgnoreCase))
            {
                QualitySettings.SetQualityLevel(i, true);
                Debug.Log($"[Graphics] Set quality to: {names[i]} (index {i})");
                return;
            }
        }

        Debug.LogWarning($"[Graphics] Quality level '{qualityName}' not found in Project Settings > Quality");
    }


    private void ApplyBrightness()
    {
        if (postProcessingVolume == null)
        {
            Debug.LogWarning("[DisplayManager] No PostProcessingVolume assigned or found.");
            return;
        }

        if (!postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogWarning("[DisplayManager] ColorAdjustments not found in post-processing profile.");
            return;
        }

        colorAdjustments.postExposure.value = gameSettings.screenBrightness;
    }

    public void UpdateSettings(GameSettingsData newSettings)
    {
        gameSettings = newSettings;
        ES3.Save(settingsKey, gameSettings);
        ApplyDisplaySettings();
    }

    public GameSettingsData GetSettings() => gameSettings;

    public void SetBrightness(float uiValue)
    {
        if (postProcessingVolume == null)
        {
            Debug.LogWarning("PostProcessingVolume is not assigned.");
            return;
        }

        if (colorAdjustments == null)
        {
            if (!postProcessingVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.LogWarning("ColorAdjustments not found in the Volume Profile.");
                return;
            }
        }

        float mappedValue = Mathf.Lerp(-5f, 5f, uiValue / 100f);
        colorAdjustments.postExposure.value = mappedValue;
        gameSettings.screenBrightness = mappedValue;

        ES3.Save(settingsKey, gameSettings);
        Debug.Log($"[DisplayManager] Brightness updated to {mappedValue}");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RefreshPostProcessing());
    }

    private IEnumerator RefreshPostProcessing()
    {
        yield return null; // vänta 1 frame för att vara säker på att allt laddats

        postProcessingVolume = FindObjectOfType<UnityEngine.Rendering.Volume>();

        if (postProcessingVolume != null)
        {
            Debug.Log($"[DisplayManager] Found volume in {SceneManager.GetActiveScene().name}");
            ApplyBrightness(); // eller andra post-effekter
        }
        else
        {
            Debug.LogWarning("[DisplayManager] Global Volume not found!");
        }
    }

}
