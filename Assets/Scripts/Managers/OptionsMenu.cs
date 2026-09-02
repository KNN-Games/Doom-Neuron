using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using System.Collections.Generic;

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
/// 1. private FloatSetting/StringSetting
/// 2. references to the UI elements that will control the setting (slider, button, etc.). This is not always nessesary.
/// 3. A function to set the new setting. Connect that function to the button you want in setting menu in Unity Editor.
/// 4. modify Start() with: settingName = new FloatSetting/StringSetting("settingName", defaultValue, FunctionThatChangesThisSetting)
/// </remarks>
public class OptionsMenu : Singleton<OptionsMenu>
{
    public bool IsOptionsMenuOpen => optionsMenuCanvas.activeSelf;
    [SerializeField] private AudioMixer audioMixer;
    [Header("Input action References")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference interactAction;
    [Header("Main UI References")]
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject generalSettingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;
    [SerializeField] private GameObject saveChangesPrompt;
    [SerializeField] private Button saveChangesConfirmButton;
    [SerializeField] private Button generalSettingsButton;
    [Header("General UI References")]
    [SerializeField] private SettingSlider fovSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    [Header("Audio UI References")]
    [SerializeField] private SettingSlider masterVolumeSlider;
    [SerializeField] private SettingSlider musicVolumeSlider;
    [SerializeField] private SettingSlider sfxVolumeSlider;
    [Header("Controls UI References")]
    [SerializeField] private LocalizeStringEvent deviceDetectedLocalizedText;
    [SerializeField] private SettingSlider mouseSensitivitySlider;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button interactButton;
    private TextMeshProUGUI jumpButtonText;
    private TextMeshProUGUI interactButtonText;
    private List<Resolution> availableResolutions; // List of available resolutions for the resolution dropdown. Populated in Start() by GetAvailableResolutions()
    private static readonly List<OptionSetting> allSettings = new(); // All settings
    // Audio settings
    private FloatSetting masterVolume;
    private FloatSetting musicVolume;
    private FloatSetting sfxVolume;
    // General settings
    private StringSetting language;
    private FloatSetting fov;
    private IntSetting resolution;
    private IntSetting windowMode; // 0 = FullScreenWindow, 1 = MaximizedWindow, 2 = Windowed
    // Keyboard Controls settings
    private FloatSetting mouseSensitivity;
    private StringSetting jumpKeyboard;
    private StringSetting interactKeyboard;
    // Gamepad Controls settings
    private StringSetting jumpGamepad;
    private StringSetting interactGamepad;
    // Other
    private PlayerInput playerInput;

    private void Start()
    {
        // Get the list of available resolutions
        GetAvailableResolutions();
        // Create all settings objects
        fov = new FloatSetting("fov", 90f, SetFOV);
        resolution = new IntSetting("resolution", GetCurrentIndex(), SetResolution);
        windowMode = new IntSetting("windowMode", (int)FullScreenMode.FullScreenWindow, SetWindowMode);
        masterVolume = new FloatSetting("masterVolume", 100f, SetMasterVolume);
        musicVolume = new FloatSetting("musicVolume", 100f, SetMusicVolume);
        sfxVolume = new FloatSetting("sfxVolume", 100f, SetSFXVolume);
        language = new StringSetting("language", "en", SetLanguage);
        mouseSensitivity = new FloatSetting("mouseSensitivity", 10f, SetMouseSensitivity);
        // Control settings
        // FindBindings returns int index from that array that holds binding with "<device>". Look in INTERNAL METHODS (line ~330) for how it works exactly.
        jumpKeyboard = new StringSetting(
            "jumpKey",
            jumpAction.action.bindings[FindBinding(jumpAction.action, "<Keyboard>")].path,
            value => jumpAction.action.ApplyBindingOverride(FindBinding(jumpAction.action, "<Keyboard>"), value));
        interactKeyboard = new StringSetting(
            "interactKey",
            interactAction.action.bindings[FindBinding(interactAction.action, "<Keyboard>")].path,
            value => interactAction.action.ApplyBindingOverride(FindBinding(interactAction.action, "<Keyboard>"), value));
        jumpGamepad = new StringSetting(
            "jumpGamepad",
            jumpAction.action.bindings[FindBinding(jumpAction.action, "<Gamepad>")].path,
            value => jumpAction.action.ApplyBindingOverride(FindBinding(jumpAction.action, "<Gamepad>"), value));
        interactGamepad = new StringSetting(
            "interactGamepad",
            interactAction.action.bindings[FindBinding(interactAction.action, "<Gamepad>")].path,
            value => interactAction.action.ApplyBindingOverride(FindBinding(interactAction.action, "<Gamepad>"), value));

        jumpButtonText = jumpButton.GetComponentInChildren<TextMeshProUGUI>();
        interactButtonText = interactButton.GetComponentInChildren<TextMeshProUGUI>();

        ApplySavedValues();
        PlayerPrefs.Save();
        playerInput = InputManager.Instance.playerInput;

        PrintAllSettingValues();
    }
    //---GENERAL SETTINGS---
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
    public void SetLanguage(string languageCode)
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
    public void SetFOV(float newFov) // The slider makes it so it is always a whole number, but internally it is still a float
    {
        fov.CurrentValue = newFov;
        GameManager.Instance.fov = newFov;
        if (PlayerController.Instance != null) // I do not use Camera.main because what if we add cutscenes and player modifies FOV mid one?
        {
            PlayerController.Instance.camera.fieldOfView = newFov;
        }
        fovSlider.UpdateSlider(newFov, newFov.ToString(), newFov != fov.DefaultValue);
    }
    //---AUDIO SETTINGS---
    public void SetMasterVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        masterVolume.CurrentValue = volume;
        masterVolumeSlider.UpdateSlider(volume, volume + "%", volume != masterVolume.DefaultValue);
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
    }
    public void SetMusicVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        musicVolume.CurrentValue = volume;
        musicVolumeSlider.UpdateSlider(volume, volume + "%", volume != musicVolume.DefaultValue);
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }
    public void SetSFXVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        sfxVolume.CurrentValue = volume;
        sfxVolumeSlider.UpdateSlider(volume, volume + "%", volume != sfxVolume.DefaultValue);
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
    }
    //---CONTROLS SETTINGS---
    public void SetMouseSensitivity(float sensitivity) // Parameter: 1-30 -> 0,1-3,0
    {
        mouseSensitivity.CurrentValue = sensitivity;
        mouseSensitivitySlider.UpdateSlider(
            sensitivity,
            (mouseSensitivity.CurrentValue / 10).ToString(),
            sensitivity != mouseSensitivity.DefaultValue);
    }
    public void RebindJump(bool isforKeyboard)
    {
        BeginRebind(jumpAction, jumpKeyboard, isforKeyboard, jumpButtonText);
    }
    public void RebindInteract(bool isforKeyboard)
    {
        BeginRebind(interactAction, interactKeyboard, isforKeyboard, interactButtonText);
    }
    //---MENU NAVIGATION---
    public void OpenOptionsMenu() // Used by PlayerUI and MainMenu to open the options menu.
    {
        optionsMenuCanvas.SetActive(true);
        generalSettingsButton.Select();
        //generalSettingsButton.Select(); // Select general settings button by default for non-mouse navigation
        OpenGeneralSettings();

        // Reset preview state to the last persisted values whenever the menu opens.
        ApplySavedValues();
    }
    public void BackToMenu()
    {
        if (HasUnsavedChanges())
        {
            saveChangesPrompt.SetActive(true);
            saveChangesConfirmButton.Select();
            return;
        }
        ExitSettings();
    }
    public void OpenGeneralSettings()
    {
        generalSettingsPanel.SetActive(true);
        audioSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(false);
        GetAvailableResolutions();
    }
    public void OpenAudioSettings()
    {
        generalSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(true);
        controlsSettingsPanel.SetActive(false);
    }
    public void OpenControlsSettings()
    {
        generalSettingsPanel.SetActive(false);
        audioSettingsPanel.SetActive(false);
        controlsSettingsPanel.SetActive(true);
        // Check what device is detected. Game only supports gamepad and keyboardMouse, so either detected gamepad or use default keyboard
        jumpButton.onClick.RemoveAllListeners();
        interactButton.onClick.RemoveAllListeners();

        Debug.Log(playerInput.currentControlScheme);
        if (playerInput.currentControlScheme == "Gamepad")
        {
            deviceDetectedLocalizedText.StringReference.TableEntryReference = "GAMEPAD DETECTED";
            jumpButton.onClick.AddListener(() => RebindJump(false));
            interactButton.onClick.AddListener(() => RebindInteract(false));
            jumpButtonText.text = GetBindingDisplayName(jumpGamepad.CurrentValue);
            interactButtonText.text = GetBindingDisplayName(interactGamepad.CurrentValue);
        }
        else
        {
            deviceDetectedLocalizedText.StringReference.TableEntryReference = "KEYBOARD AND MOUSE DETECTED";
            jumpButton.onClick.AddListener(() => RebindJump(true));
            interactButton.onClick.AddListener(() => RebindInteract(true));
            jumpButtonText.text = GetBindingDisplayName(jumpKeyboard.CurrentValue);
            interactButtonText.text = GetBindingDisplayName(interactKeyboard.CurrentValue);
        }
        deviceDetectedLocalizedText.RefreshString();
    }
    public void ResetSettings() // Reset all settings
    {
        foreach (var setting in allSettings)
        {
            setting.ResetToDefault();
            setting.InvokeChangeSettingFunction();
        }
        SaveChanges();
        BackToMenu();
        Debug.Log("Resetting settings to default values");
    }
    public void ResetSetting(string settingName)
    {
        foreach (var setting in allSettings)
        {
            if (setting.Key != settingName) continue;
            setting.ResetToDefault();
            setting.InvokeChangeSettingFunction();
            return;
        }
        Debug.LogError("Setting not found");
    }
    public void Confirm()
    {
        SaveChanges();
        ExitSettings();
    }
    public void Discard()
    {
        ApplySavedValues();
        ExitSettings();
    }
    public void Cancel()
    {
        saveChangesPrompt.SetActive(false);
    }
    private void ExitSettings()
    {
        saveChangesPrompt.SetActive(false);
        optionsMenuCanvas.SetActive(false);
        if (MainMenu.Instance != null) // if in main menu reactivate it.
        {
            MainMenu.Instance.ReturnToMainMenu();
        }
    }
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
    private int FindBinding(InputAction action, string device)
    {
        for (int i = 0; i < action.bindings.Count; i++)
        {
            string path = action.bindings[i].path;
            if (path.StartsWith(device)) return i;
        }
        Debug.LogError("Input action path not found!");
        return -1;
    }
    // ADD: function to update all rebind button values
    private string GetBindingDisplayName(string bindingPath)
    {
        return bindingPath.Replace("<Keyboard>/", string.Empty).Replace("<Gamepad>/", string.Empty).ToUpper();
    }
    private void BeginRebind(InputAction action, StringSetting setting, bool isForKeyboard, TextMeshProUGUI buttonText)
    {
        string device = string.Empty;
        string cancelKey = string.Empty;
        if (isForKeyboard)
        {
            device = "<Keyboard>";
            cancelKey = "<Keyboard>/escape";
        }
        else // assume it's for Gamepad
        {
            device = "<Gamepad>";
            cancelKey = "<Gamepad>/start";
        }
        int bindingIndex = FindBinding(action, device);
        if (bindingIndex < 0) return;

        action.Disable();

        // Oh my goodness gracious
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation.html
        action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(device)
            .WithCancelingThrough(cancelKey)
            .OnComplete(operation =>
            {
                string newBinding = action.bindings[bindingIndex].overridePath;
                setting.CurrentValue = newBinding;
                Debug.Log($"{action.name} rebound to {newBinding}");
                buttonText.text = GetBindingDisplayName(newBinding);
                operation.Dispose();
                action.Enable();
            })
            .OnCancel(operation =>
            {
                operation.Dispose();
                action.Enable();
                Debug.Log("Rebinding cancelled.");
            })
            .Start();
    }
    private void GetAvailableResolutions() // Update the availableResolutions list (with available resolutions) and the resolution dropdown options
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
        Debug.Log(
        $"Settings loaded. Values:\n" +
        $"Master Volume: {masterVolume.SavedValue}\n" +
        $"Music Volume: {musicVolume.SavedValue}\n" +
        $"SFX Volume: {sfxVolume.SavedValue}\n" +
        $"Language: {language.SavedValue}\n" +
        $"Mouse Sensitivity: {mouseSensitivity.SavedValue}\n" +
        $"jumpKeyboard: {jumpKeyboard.SavedValue}\n" +
        $"interactKeyboard: {interactKeyboard.SavedValue}\n" +
        $"jumpGamepad: {jumpGamepad.SavedValue}\n" +
        $"interactGamepad: {interactGamepad.SavedValue}\n" +
        $"FOV: {fov.SavedValue}\n" +
        $"Resolution: {resolution.SavedValue}\n" +
        $"Window Mode: {windowMode.SavedValue}\n");
    }
    //--- SETTING CLASSES ---
    private abstract class OptionSetting
    {
        public readonly string Key;
        protected OptionSetting(string key)
        {
            Key = key;
        }
        public abstract void InvokeChangeSettingFunction();
        public abstract void ApplySavedToCurrent();
        public abstract void ApplyCurrentToSaved(); // As in: save new values
        public abstract void WriteToPlayerPrefs();
        public abstract void ResetToDefault();
        public abstract bool Changed(); // As in: is value changed?
    }
    private class StringSetting : OptionSetting
    {
        private readonly System.Action<string> changeSettingFunction;
        public readonly string DefaultValue;
        public string CurrentValue;
        public string SavedValue;
        public StringSetting(string key, string defaultValue, System.Action<string> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetString(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetString(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetString(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !string.Equals(SavedValue, CurrentValue);
        }
    }
    private class FloatSetting : OptionSetting
    {
        private readonly System.Action<float> changeSettingFunction;
        public readonly float DefaultValue;
        public float CurrentValue;
        public float SavedValue;
        public FloatSetting(string key, float defaultValue, System.Action<float> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetFloat(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetFloat(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetFloat(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !Mathf.Approximately(CurrentValue, SavedValue);
        }
    }
    private class IntSetting : OptionSetting
    {
        private readonly System.Action<int> changeSettingFunction;
        public readonly int DefaultValue;
        public int CurrentValue;
        public int SavedValue;
        public IntSetting(string key, int defaultValue, System.Action<int> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetInt(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetInt(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetInt(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return CurrentValue != SavedValue;
        }
    }
}