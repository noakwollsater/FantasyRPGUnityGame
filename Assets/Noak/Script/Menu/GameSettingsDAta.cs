[System.Serializable]
public class GameSettingsData
{
    public bool firstTime = true;
    public bool acceptLicense = false;
    public float screenBrightness = 1.0f;

    public float masterVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float dialogueVolume = 1.0f;

    public string dynamicRange = "Eyes bleeding";
    public string musicFrequency = "Slow Dance";

    public bool cinematicSubtitles = true;
    public bool gameplaySubtitles = true;
    public float textSize = 1.0f;
    public float opacity = 1.0f;
    // Add more settings as needed (e.g., musicVolume, sfxVolume, etc.)
}
