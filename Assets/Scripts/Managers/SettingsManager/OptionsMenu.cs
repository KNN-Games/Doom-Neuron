using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Handles the options menu, including audio, controls, and general settings.
/// </summary>
/// <remarks>
/// Each setting is an object that stores DEFAULT, SAVED and CURRENT values that can be a float or a string (it is also possible to implement a int setting)
/// DEFAULT values are used when first lauching the game and when resetting settings. They are set during setting creation and cannot be changed later.
/// SAVED values are what is already saved to disk (used to discard changes and check for unsaved changes).
/// CURRENT values are "live-preview" - temporary value that can be saved or discard. They are applied immediately when changing a setting (this is most important for audio setting)
/// When you change a slider/button/whatever you should feel the effects immediately, but not save them until you hit the save button.
/// 
/// When adding a new setting you have to add:
/// 1. private FloatSetting/StringSetting/IntSetting/RebindSetting
/// 2. references to the UI elements that will control the setting (slider, button, etc.). This is not always nessesary.
/// 3. A function to set the new setting. Connect that function to the button you want in setting menu in Unity Editor.
/// 4. modify Start() with: settingName = new FloatSetting/StringSetting/IntSetting("settingName", defaultValue, FunctionThatChangesThisSetting)
/// It's a bit different with rebind settings: only call CreateBindingSettings(inputActionReference.action, "name", rebindButton);
/// </remarks>
public partial class OptionsMenu : Singleton<OptionsMenu>
{
    public bool IsOptionsMenuOpen => optionsMenuCanvas.activeSelf;
    [SerializeField] private AudioMixer audioMixer;
    [Header("Input action References")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference pauseAction;
    [Header("URP asset reference")]
    [SerializeField] private UniversalRenderPipelineAsset urpAsset;
    [Header("Main UI References")]
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject generalSettingsPanel;
    [SerializeField] private GameObject graphicsSettingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;
    [SerializeField] private GameObject saveChangesPrompt;
    [SerializeField] private Button saveChangesConfirmButton;
    [SerializeField] private Button generalSettingsButton;
    [Header("Graphics UI References")]
    [SerializeField] private SettingSlider fovSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    [SerializeField] private SettingSlider renderScaleSlider;
    [Header("Audio UI References")]
    [SerializeField] private SettingSlider masterVolumeSlider;
    [SerializeField] private SettingSlider musicVolumeSlider;
    [SerializeField] private SettingSlider sfxVolumeSlider;
    [Header("Controls UI References")]
    [SerializeField] private LocalizeStringEvent changeBindingsLocalizedText;
    [SerializeField] private LocalizeStringEvent deviceDetectedLocalizedText;
    [SerializeField] private GameObject keyboardSection;
    [SerializeField] private GameObject gamepadSection;
    [SerializeField] private SettingSlider mouseSensitivitySlider;
    [SerializeField] private SettingSlider gamepadSensitivitySlider;
    [SerializeField] private Toggle gamepadInvertYToggle;
    // Rebind section
    [SerializeField] private RebindButton jumpButton;
    [SerializeField] private RebindButton interactButton;
    [SerializeField] private RebindButton pauseButton; // as in: pause rebind button
    private Coroutine renderScaleCoroutine;
    private static readonly WaitForSecondsRealtime _waitForSeconds0_5 = new(0.5f);
    private List<Resolution> availableResolutions; // List of available resolutions for the resolution dropdown. Populated in Start() by UpdateAvailableResolutions()
    private static readonly List<RebindSetting> rebinds = new(); // List of all rebinds.
    private static readonly List<OptionSetting> allSettings = new(); // All settings
    private PlayerInput playerInput;
    // Audio settings
    private FloatSetting masterVolume;
    private FloatSetting musicVolume;
    private FloatSetting sfxVolume;
    // General settings
    private StringSetting language;
    // Graphics settings
    private FloatSetting fov;
    private IntSetting resolution;
    private IntSetting windowMode; // 0 = FullScreenWindow, 1 = MaximizedWindow, 2 = Windowed
    private FloatSetting renderScale;
    // Keyboard Controls settings
    private FloatSetting mouseSensitivity;
    // Gamepad Controls settings
    private FloatSetting gamepadSensitivity;
    private IntSetting gamepadInvertY; // 1 = on, 0 = off

    private void Start()
    {
        // Get the list of available resolutions
        UpdateAvailableResolutions();
        // Create all settings objects
        fov = new FloatSetting("fov", 90f, SetFOV);
        resolution = new IntSetting("resolution", GetCurrentIndex(), SetResolution);
        windowMode = new IntSetting("windowMode", (int)FullScreenMode.FullScreenWindow, SetWindowMode);
        renderScale = new FloatSetting("renderScale", 100f, SetRenderScale);
        masterVolume = new FloatSetting("masterVolume", 100f, SetMasterVolume);
        musicVolume = new FloatSetting("musicVolume", 100f, SetMusicVolume);
        sfxVolume = new FloatSetting("sfxVolume", 100f, SetSFXVolume);
        language = new StringSetting("language", "en", SetLanguage);
        mouseSensitivity = new FloatSetting("mouseSensitivity", 10f, SetMouseSensitivity);
        gamepadSensitivity = new FloatSetting("gamepadSensitivity", 10f, SetGamepadSensitivity);
        gamepadInvertY = new IntSetting("gamepadInvertY", 0, SetInvertedLookDirection);
        // Control settings
        CreateBindingSettings(jumpAction.action, "jump", jumpButton);
        CreateBindingSettings(interactAction.action, "interact", interactButton);
        CreateBindingSettings(pauseAction.action, "pause", pauseButton);

        ApplySavedValues();
        PlayerPrefs.Save();
        playerInput = InputManager.Instance.playerInput;
    }
    //---GENERAL SETTINGS---
    public void SetLanguage(string languageCode) // Parameter: language code - "pl" or "en"
    {
        language.CurrentValue = languageCode;
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                return;
            }
        }
        Debug.LogError("Language not found");
    }
    //---GRAPHICS SETTINGS---
    public void SetResolution(int index) // Parameter: index of the Resolution in availableResolutions list
    {
        if (index < 0 || index >= availableResolutions.Count)
        {
            Debug.LogError($"Wrong index: {index}");
            return;
        }
        resolution.CurrentValue = index;
        var res = availableResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        resolutionDropdown.SetValueWithoutNotify(index);
    }
    public void SetWindowMode(int index) // Parameter: 0 = full screen, 1 = maximized, 2 = windowed
    {
        windowMode.CurrentValue = index;
        // in FullScreenMode: 1 = FullScreenWindow, 2 = MaximizedWindow, 3 = Windowed, so I need to add 1 to the index to match the enum values
        Screen.fullScreenMode = (FullScreenMode)(index + 1);
        windowModeDropdown.SetValueWithoutNotify(index);
    }
    public void SetRenderScale(float percent) // Parameter: 10-200 -> 0.1-2.0
    {
        // To prevent the render scale from being set too often, we use a coroutine to delay the actual setting of the render scale. 
        // This is because changing the render scale can be expensive and we don't want to do it on every slider change.
        renderScale.CurrentValue = percent;
        renderScaleSlider.UpdateSlider(percent, percent + "%", renderScale.NotDefault());
        if (renderScaleCoroutine != null)
        {
            StopCoroutine(renderScaleCoroutine);
        }
        renderScaleCoroutine = StartCoroutine(SetRenderScaleCoroutine(percent));
    }
    private IEnumerator SetRenderScaleCoroutine(float percent)
    {
        yield return _waitForSeconds0_5;
        ApplyRenderScale(percent);
    }
    private void ApplyRenderScale(float percent)
    {
        if (urpAsset != null)
        {
            urpAsset.renderScale = percent / 100f;
        }
        else
        {
            Debug.LogError("URP Asset is not assigned in OptionsMenu.");
        }
    }
    public void SetFOV(float newFov) // Parameter: 30-120
    {
        // The slider makes it so it is always a whole number, but internally it is still a float
        fov.CurrentValue = newFov;
        GameManager.Instance.fov = newFov;
        if (PlayerController.Instance != null) // I do not use Camera.main because what if we add cutscenes and player modifies FOV mid one?
        {
            PlayerController.Instance.camera.fieldOfView = newFov;
        }
        fovSlider.UpdateSlider(newFov, newFov.ToString(), fov.NotDefault());
    }
    //---AUDIO SETTINGS---
    public void SetMasterVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        masterVolume.CurrentValue = volume;
        masterVolumeSlider.UpdateSlider(volume, volume + "%", masterVolume.NotDefault());
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
    }
    public void SetMusicVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        musicVolume.CurrentValue = volume;
        musicVolumeSlider.UpdateSlider(volume, volume + "%", musicVolume.NotDefault());
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }
    public void SetSFXVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        sfxVolume.CurrentValue = volume;
        sfxVolumeSlider.UpdateSlider(volume, volume + "%", sfxVolume.NotDefault());
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
    }
    //---CONTROLS SETTINGS---
    //TO DO: merge these two?
    public void SetMouseSensitivity(float sensitivity) // Parameter: 1-30 -> 0,1-3,0
    {
        mouseSensitivity.CurrentValue = sensitivity;
        mouseSensitivitySlider.UpdateSlider(sensitivity, (mouseSensitivity.CurrentValue / 10).ToString(), mouseSensitivity.NotDefault());
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.mouseSensitivity = sensitivity / 10;
            PlayerController.Instance.UpdateSensitivity();
        }
    }
    public void SetGamepadSensitivity(float sensitivity) // Parameter: 1-30 -> 0,1-3,0
    {
        gamepadSensitivity.CurrentValue = sensitivity;
        gamepadSensitivitySlider.UpdateSlider(sensitivity, (gamepadSensitivity.CurrentValue / 10).ToString(), gamepadSensitivity.NotDefault());
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.gamepadSensitivity = sensitivity / 10;
            PlayerController.Instance.UpdateSensitivity();
        }
    }
    public void SetInvertedLookDirection(int value) // 0 = off, 1 = on
    {
        gamepadInvertY.CurrentValue = value;
        gamepadInvertYToggle.SetIsOnWithoutNotify(value == 1);
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.invertGamepadY = value == 1;
            PlayerController.Instance.UpdateSensitivity();
        }
    }
    public void SetInvertedLookDirection(bool isOn)
    {
        SetInvertedLookDirection(isOn ? 1 : 0);
    }
    // Rebinds are made entirely via AddListener, so they don't need to be written here
    //---INTERNAL FUNCTIONS---
    private void ApplySavedValues() // Update settings & UI to match SAVED values.
    {
        foreach (var setting in allSettings)
        {
            setting.ApplySavedToCurrent();
            setting.InvokeChangeSettingFunction();
        }
    }
    public void SaveChanges() // Used then player confirms changes in prompt OR when player hits save button in options menu
    {
        // Stop all coroutines that are waiting to a setting, because we want to save the current value immediately.
        if (renderScaleCoroutine != null)
        {
            StopCoroutine(renderScaleCoroutine);
            ApplyRenderScale(renderScale.CurrentValue);
        }
        foreach (var setting in allSettings)
        {
            setting.ApplyCurrentToSaved();
        }
        foreach (var setting in allSettings)
        {
            setting.WriteToPlayerPrefs();
        }
        PlayerPrefs.Save();
    }
    private bool HasUnsavedChanges()
    {
        foreach (var setting in allSettings)
        {
            if (setting.Changed())
            {
                return true;
            }
        }
        return false;
    }
    private void UpdateAvailableResolutions() // Update the availableResolutions list (with available resolutions) and the resolution dropdown options
    {
        var seen = new HashSet<(int, int)>(); // To keep track of unique width/height pairs
        availableResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        var labels = new List<string>(); // the labels for the dropdown options
        // Screen.resolutions returns all available resolutions, including duplicates. We want to filter out duplicates and only keep unique width/height pairs.
        foreach (var res in Screen.resolutions)
        {
            var key = (res.width, res.height);
            if (seen.Contains(key)) continue; //  Discard duplicate resolutions
            seen.Add(key);
            availableResolutions.Add(res); // Add the unique resolution to the list
            labels.Add($"{res.width} x {res.height}");
        }
        resolutionDropdown.AddOptions(labels);
        // Set the dropdown to the current resolution.
        for (int i = 0; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].width == Screen.currentResolution.width && availableResolutions[i].height == Screen.currentResolution.height)
            {
                resolutionDropdown.SetValueWithoutNotify(i);
                break;
            }
        }
    }
    private int GetCurrentIndex()
    {
        return availableResolutions.FindIndex(r => r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height);
    }
    public void PrintAllSettingValues()
    {
        var sb = new System.Text.StringBuilder("Settings loaded. Values:\n");
        foreach (var setting in allSettings)
        {
            sb.AppendLine(setting.DebugLine());
        }
        Debug.Log(sb.ToString());
    }
}