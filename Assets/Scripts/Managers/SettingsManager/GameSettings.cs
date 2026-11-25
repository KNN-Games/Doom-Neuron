using UnityEngine;

[System.Serializable]
public class GameSettings
{
    // General
    public string language = ""; // Empty language means "use system language"

    // Audio (0-1, the UI converts to percent)
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;

    // Controls
    public float mouseSensitivity = 1f;
    public float gamepadSensitivity = 1f;
    public bool gamepadInvertY = false;
    public string bindingOverridesJson = ""; // Empty = no overrides. Filled by InputActionAsset.SaveBindingOverridesAsJson()

    // Graphics
    public float fov = 90f;
    public float renderScale = 1f; // 0.1-2.0
    public int resolutionWidth = 0;  // 0 = use native resolution
    public int resolutionHeight = 0;
    public FullScreenMode windowMode = FullScreenMode.FullScreenWindow;

    public GameSettings Clone() => JsonUtility.FromJson<GameSettings>(JsonUtility.ToJson(this));

    public bool IsSameAs(GameSettings other) => JsonUtility.ToJson(this) == JsonUtility.ToJson(other);

    // Keeps hand-edited or corrupted values in a sane range. Call after loading and before saving
    public void Validate()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        musicVolume = Mathf.Clamp01(musicVolume);
        sfxVolume = Mathf.Clamp01(sfxVolume);
        mouseSensitivity = Mathf.Clamp(mouseSensitivity, 0.1f, 3f);
        gamepadSensitivity = Mathf.Clamp(gamepadSensitivity, 0.1f, 3f);
        fov = Mathf.Clamp(fov, 30f, 120f);
        renderScale = Mathf.Clamp(renderScale, 0.1f, 2f);
        resolutionWidth = Mathf.Max(0, resolutionWidth);
        resolutionHeight = Mathf.Max(0, resolutionHeight);
        windowMode = (FullScreenMode)Mathf.Clamp((int)windowMode, (int)FullScreenMode.FullScreenWindow, (int)FullScreenMode.Windowed);
    }
}
