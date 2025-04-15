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

    public bool showTutorial = true;
    public float controllervibration = 1.0f;

    public bool autoSave = true;

    public string aimassist = "Option B";
    public bool wallhackz = false;

    public string graphicsQuality = "Performance";
    public string displaymode = "Fullscreen";

    public float contrast = 1.0f;
    public string resolution = "1920x1080";

    public string ambientOcclusion = "SSAO";
    public string textureQuality = "Low";
    public string textureFiltering = "4xAnisotropic";
    public string waterQuality = "low";
    public string refreshRate = "60Hz";
    public bool vsync = true;
    public bool backgroundcryptomining = true;
    public bool raytracedreflections = true;
    public bool motionblur = true;
    public bool depthoffield = true;
    public bool early2000gameweefilter = true;
    public bool bloom = true;

    public string audiolanguage = "Igpay Atinlay";
    public string subtitleslanguage = "Gen Alpha Brainrot";
    public string gameplaylanguage = "Igpay Atinlay";
    // Add more settings as needed (e.g., musicVolume, sfxVolume, etc.)
}
