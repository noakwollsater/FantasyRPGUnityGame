using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource[] musicSources;
    public AudioSource[] sfxSources;
    public AudioSource[] dialogueSources;

    private GameSettingsData gameSettings;
    private const string settingsKey = "GameSettings";

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Delay slightly to ensure all objects are initialized
        Invoke(nameof(FindAndReassignAudioSources), 0.1f);
    }

    private void FindAndReassignAudioSources()
    {
        // You can tag all relevant GameObjects, or search by name
        musicSources = FindAudioSourcesByTagOrName("Music");
        sfxSources = FindAudioSourcesByTagOrName("Ambient");
        dialogueSources = FindAudioSourcesByTagOrName("Dialogue");

        ApplySoundSettings();
    }

    private AudioSource[] FindAudioSourcesByTagOrName(string identifier)
    {
        List<AudioSource> foundSources = new List<AudioSource>();

        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains(identifier)) // Removed CompareTag
            {
                AudioSource src = obj.GetComponent<AudioSource>();
                if (src != null)
                    foundSources.Add(src);
            }
        }

        return foundSources.ToArray();
    }


    private void LoadSettings()
    {
        if (ES3.KeyExists(settingsKey))
            gameSettings = ES3.Load<GameSettingsData>(settingsKey);
        else
            gameSettings = new GameSettingsData();
    }

    public void ReassignAudioSources(AudioSource[] newMusic, AudioSource[] newSFX, AudioSource[] newDialogue)
    {
        musicSources = newMusic;
        sfxSources = newSFX;
        dialogueSources = newDialogue;

        ApplySoundSettings();
    }

    public void ApplySoundSettings()
    {
        float master = Mathf.Clamp01(gameSettings.masterVolume);

        if (musicSources != null)
        {
            foreach (var src in musicSources)
                if (src != null)
                    src.volume = Mathf.Clamp01(gameSettings.musicVolume) * master;
        }

        if (sfxSources != null)
        {
            foreach (var src in sfxSources)
                if (src != null)
                    src.volume = Mathf.Clamp01(gameSettings.sfxVolume) * master;
        }

        if (dialogueSources != null)
        {
            foreach (var src in dialogueSources)
                if (src != null)
                    src.volume = Mathf.Clamp01(gameSettings.dialogueVolume) * master;
        }
    }

    public void UpdateSettings(GameSettingsData updatedSettings)
    {
        gameSettings = updatedSettings;
        ApplySoundSettings();
        ES3.Save(settingsKey, gameSettings);
    }

    public GameSettingsData GetSettings() => gameSettings;
}
