using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Handles the options menu, including audio, controls, and general settings.
/// </summary>
/// 
/// Each setting is an object that stores DEFAULT, SAVED and CURRENT values that can be a float or a string (it is also possible to implement a int setting)
/// DEFAULT values are used when first lauching the game and when resetting settings. They are set during setting creation and cannot be changed later.
/// SAVED values are what is already saved to disk (used to discard changes and check for unsaved changes).
/// CURRENT values are "live-preview" - temporary value that can be saved or discard. They are applied immediately when changing a setting (this is most important for audio setting)
/// When you change a slider/button/whatever you should feel the effects immediately, but not save them until you hit the save button.
/// 
/// When adding a new setting you have to add:
/// 1. private FloatSetting/StringSetting
/// 2. references to the UI elements that will control the setting (slider, button, etc.). This is not always nessesary.
/// 3. modify Start() with: settingName = new FloatSetting/StringSetting("settingName", defaultValue)
/// 4. modify ApplySavedValues() to set the CURRENT value to match the SAVED and update UI if nessesary
/// 5. A function to set the new setting. Connect that function to the button you want in setting menu in Unity Editor.
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
    [SerializeField] private Slider fovSlider;
    [SerializeField] private TextMeshProUGUI fovText;
    [Header("Audio UI References")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [Header("Controls UI References")]
    [SerializeField] private LocalizeStringEvent deviceDetectedLocalizedText;
    [SerializeField] private TextMeshProUGUI mouseSensitivityText;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button interactButton;
    private TextMeshProUGUI jumpButtonText;
    private TextMeshProUGUI interactButtonText;
    // All settings
    private static readonly System.Collections.Generic.List<OptionSetting> allSettings = new();
    // Audio settings
    private FloatSetting masterVolume;
    private FloatSetting musicVolume;
    private FloatSetting sfxVolume;
    // General settings
    private StringSetting language;
    private FloatSetting fov;
    // Keyboard Controls settings
    private FloatSetting mouseSensitivity;
    private StringSetting jumpKeyboard;
    private StringSetting interactKeyboard;
    // Gamepad Controls settings
    private StringSetting jumpGamepad;
    private StringSetting interactGamepad;

    private void Start()
    {
        fov = new FloatSetting("fov", 90f);
        masterVolume = new FloatSetting("masterVolume", 100f);
        musicVolume = new FloatSetting("musicVolume", 100f);
        sfxVolume = new FloatSetting("sfxVolume", 100f);
        language = new StringSetting("language", "en");
        mouseSensitivity = new FloatSetting("mouseSensitivity", 10f);
        // Control settings
        // FindBindings returns int index from that array that holds binding with "<device>". Look in INTERNAL METHODS (line ~330) for how it works exactly.
        jumpKeyboard = new StringSetting("jumpKey", jumpAction.action.bindings[FindBinding(jumpAction.action, "<Keyboard>")].path);
        interactKeyboard = new StringSetting("interactKey", interactAction.action.bindings[FindBinding(interactAction.action, "<Keyboard>")].path);
        jumpGamepad = new StringSetting("jumpGamepad", jumpAction.action.bindings[FindBinding(jumpAction.action, "<Gamepad>")].path);
        interactGamepad = new StringSetting("interactGamepad", interactAction.action.bindings[FindBinding(interactAction.action, "<Gamepad>")].path);

        jumpButtonText = jumpButton.GetComponentInChildren<TextMeshProUGUI>();
        interactButtonText = interactButton.GetComponentInChildren<TextMeshProUGUI>();

        ApplySavedValues();
        PlayerPrefs.Save();

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
            $"interactGamepad: {interactGamepad.SavedValue}\n");

    }
    private void ApplySavedValues() // Update settings & UI to match SAVED values.
    {
        SetFOV(fov.SavedValue);
        SetMasterVolume(masterVolume.SavedValue);
        SetMusicVolume(musicVolume.SavedValue);
        SetSFXVolume(sfxVolume.SavedValue);
        SetLanguage(language.SavedValue);
        SetMouseSensitivity(mouseSensitivity.SavedValue);
        jumpAction.action.ApplyBindingOverride(FindBinding(jumpAction.action, "<Keyboard>"), jumpKeyboard.SavedValue);
        interactAction.action.ApplyBindingOverride(FindBinding(interactAction.action, "<Keyboard>"), interactKeyboard.SavedValue);
        jumpAction.action.ApplyBindingOverride(FindBinding(jumpAction.action, "<Gamepad>"), jumpGamepad.SavedValue);
        interactAction.action.ApplyBindingOverride(FindBinding(interactAction.action, "<Gamepad>"), interactGamepad.SavedValue);

        fovSlider.value = fov.SavedValue;
        masterVolumeSlider.value = masterVolume.SavedValue;
        musicVolumeSlider.value = musicVolume.SavedValue;
        sfxVolumeSlider.value = sfxVolume.SavedValue;
        mouseSensitivitySlider.value = mouseSensitivity.SavedValue;
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
        // This could be made more profesionally through control schemes, but that would need for this script to have a reference to PlayerInput component.
        // Which is on the player. Which does not exist in the main menu! So too bad!
        jumpButton.onClick.RemoveAllListeners();
        interactButton.onClick.RemoveAllListeners();
        if (DeviceObserver.Instance.ActiveDeviceType == InputDeviceType.Gamepad)
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
    public void ResetSettings()
    {
        Debug.Log("Resetting settings to default values");
        foreach (var setting in allSettings)
        {
            setting.ResetToDefault();
        }
        ApplySavedValues();
        SaveChanges();
        BackToMenu();
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
    //---GENERAL SETTINGS---
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
        fovText.text = newFov.ToString();
    }
    //---AUDIO SETTINGS---
    public void SetMasterVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        masterVolume.CurrentValue = volume;
        masterVolumeText.text = volume + "%";
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
    }
    public void SetMusicVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        musicVolume.CurrentValue = volume;
        musicVolumeText.text = volume + "%";
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }
    public void SetSFXVolume(float volume) // Parameter: 0-100 -> 0%-100%
    {
        sfxVolume.CurrentValue = volume;
        sfxVolumeText.text = volume + "%";
        float dB = Mathf.Log10(Mathf.Max(volume / 100, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
    }
    //---CONTROLS SETTINGS---
    public void SetMouseSensitivity(float sensitivity) // Parameter: 1-30 -> 0,1-3,0
    {
        mouseSensitivity.CurrentValue = sensitivity / 10;
        mouseSensitivityText.text = (mouseSensitivity.CurrentValue / 10).ToString();
    }
    public void RebindJump(bool isforKeyboard)
    {
        BeginRebind(jumpAction, jumpKeyboard, isforKeyboard, jumpButtonText);
    }
    public void RebindInteract(bool isforKeyboard)
    {
        BeginRebind(interactAction, interactKeyboard, isforKeyboard, interactButtonText);
    }
    //---INTERNAL FUNCTIONS---
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
    //--- SETTING CLASSES ---
    private abstract class OptionSetting
    {
        public abstract void ApplyCurrentToSaved(); // As in: save new values
        public abstract void WriteToPlayerPrefs();
        public abstract void ResetToDefault();
        public abstract bool Changed(); // As in: is value changed?
    }
    private class StringSetting : OptionSetting
    {
        private readonly string Key;
        private readonly string DefaultValue;
        public string CurrentValue;
        public string SavedValue;
        public StringSetting(string key, string defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
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
            SavedValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !string.Equals(SavedValue, CurrentValue);
        }
    }
    private class FloatSetting : OptionSetting
    {
        private readonly string Key;
        private readonly float DefaultValue;
        public float CurrentValue;
        public float SavedValue;
        public FloatSetting(string key, float defaultValue)
        {
            Key = key;
            DefaultValue = defaultValue;
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
            SavedValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !Mathf.Approximately(CurrentValue, SavedValue);
        }
    }
}
