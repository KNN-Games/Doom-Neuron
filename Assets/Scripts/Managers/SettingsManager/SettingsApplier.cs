using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Applies GameSettings to everything that is not a single scene object.
/// Lives on the GameManager prefab. Listens to SettingsService.Changed.
/// </summary>
public class SettingsApplier : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private UniversalRenderPipelineAsset urpAsset;
    [SerializeField] private InputActionAsset inputActions; // The same asset that PlayerInput uses

    // Last applied values, so we do not repeat expensive calls when nothing changed
    private int lastWidth = -1;
    private int lastHeight = -1;
    private FullScreenMode lastWindowMode;
    private string lastBindings;

    private void OnEnable() => SettingsService.Instance.Changed += Apply;
    private void OnDisable() => SettingsService.Instance.Changed -= Apply;

    private void Start()
    {
        Apply(SettingsService.Instance.Current, false);
    }

    private void Apply(GameSettings setting, bool isPreview)
    {
        // Settings with live-preview
        ApplyVolume("MasterVolume", setting.masterVolume);
        ApplyVolume("MusicVolume", setting.musicVolume);
        ApplyVolume("SoundEffectsVolume", setting.sfxVolume);
        ApplyLanguage(setting.language);
        ApplyBindings(setting.bindingOverridesJson);

        if (isPreview) return;

        // Settings exempt from live-preview
        ApplyScreen(setting);
        urpAsset.renderScale = setting.renderScale; // too laggy for preview
    }

    private void ApplyVolume(string mixerParameter, float volume) // volume: 0-1
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f; // 0 would give -infinity, so we clamp
        audioMixer.SetFloat(mixerParameter, dB);
    }

    private void ApplyLanguage(string languageCode)
    {
        // Empty means "not chosen yet": fall back to the system language
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = Application.systemLanguage == SystemLanguage.Polish ? "pl" : "en";
        }
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code != languageCode) continue;
            if (LocalizationSettings.SelectedLocale != locale) LocalizationSettings.SelectedLocale = locale;
            return;
        }
        Debug.LogError($"Language not found: {languageCode}");
    }

    private void ApplyBindings(string json)
    {
        if (json == lastBindings) return; // Nothing changed
        lastBindings = json;
        if (string.IsNullOrEmpty(json)) inputActions.RemoveAllBindingOverrides(); // Empty = default keys
        else inputActions.LoadBindingOverridesFromJson(json);
    }

    private void ApplyScreen(GameSettings s)
    {
        // 0 means "use native resolution"
        int width = s.resolutionWidth > 0 ? s.resolutionWidth : Display.main.systemWidth;
        int height = s.resolutionHeight > 0 ? s.resolutionHeight : Display.main.systemHeight;
        if (width == lastWidth && height == lastHeight && s.windowMode == lastWindowMode) return; // Avoids screen flicker on every Save

        Screen.SetResolution(width, height, s.windowMode);
        lastWidth = width;
        lastHeight = height;
        lastWindowMode = s.windowMode;
    }
}