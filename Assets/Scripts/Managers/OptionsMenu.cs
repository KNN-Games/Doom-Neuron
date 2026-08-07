using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// Handles the options menu, including audio, controls, and general settings.
/// CURRENT values are the live preview state while the options menu is open,
/// SAVED values are what is already saved to disk (used to discard changes and check for unsaved changes).
/// When you change a slider/button/whatever you should feel the effects immediately, but not save them until you hit the save button.
/// 
/// When adding a new setting you have to add:
/// 1. two (2) variables to hold the CURRENT and SAVED values of the setting.
/// 2. references to the UI elements that will control the setting (slider, button, etc.).
/// 3. a function to set the CURRENT value of the setting.
/// 4. modify Start() to check and load the setting from PlayerPrefs
/// 5. modify ApplySavedValues() to set the CURRENT value to match the SAVED and update UI if nessesary
/// 6. modify HasUnsavedChanges() to check if new CURRENT == SAVED
/// 7. modify SaveChanges() to set SAVED to match CURRENT and send it to PlayerPrefs
/// 8. modify ResetSettings to include default value
/// 
/// TO DO: include arrays? References are starting to get really long...
/// </summary>
public class OptionsMenu : Singleton<OptionsMenu>
{
    public bool IsOptionsMenuOpen => optionsMenuCanvas.activeSelf;
    public AudioMixer audioMixer;
    [Header("Main UI References")]
    [SerializeField] private GameObject optionsMenuCanvas;
    [SerializeField] private GameObject generalSettingsPanel;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject controlsSettingsPanel;
    [SerializeField] private GameObject saveChangesPrompt;
    [SerializeField] private Button generalSettingsButton;
    [Header("Audio UI References")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [Header("Controls UI References")]
    [SerializeField] private TextMeshProUGUI mouseSensitivityText;
    [SerializeField] private Slider mouseSensitivitySlider;
    // All settings
    private static readonly System.Collections.Generic.List<OptionSetting> allSettings = new();
    private FloatSetting masterVolume;
    private FloatSetting musicVolume;
    private FloatSetting sfxVolume;
    private StringSetting language;
    private FloatSetting mouseSensitivity;
    private StringSetting jumpKey;
    private StringSetting interactKey;

    private void Start()
    {
        masterVolume = new FloatSetting("masterVolume", 1f);
        musicVolume = new FloatSetting("musicVolume", 1f);
        sfxVolume = new FloatSetting("sfxVolume", 1f);
        language = new StringSetting("language", "en");
        mouseSensitivity = new FloatSetting("mouseSensitivity", 1f);
        jumpKey = new StringSetting("jumpKey", string.Empty);
        interactKey = new StringSetting("interactKey", string.Empty);

        allSettings.Clear();
        ApplySavedValues();
        PlayerPrefs.Save();

        Debug.Log(
            $"Settings loaded. Values:\n" +
            $"Master Volume: {masterVolume.SavedValue}\n" +
            $"Music Volume: {musicVolume.SavedValue}\n" +
            $"SFX Volume: {sfxVolume.SavedValue}\n" +
            $"Language: {language.SavedValue}\n" +
            $"Mouse Sensitivity: {mouseSensitivity.SavedValue}");
    }
    private void ApplySavedValues() // Update settings & UI to match SAVED values.
    {
        SetMasterVolume(masterVolume.SavedValue);
        SetMusicVolume(musicVolume.SavedValue);
        SetSFXVolume(sfxVolume.SavedValue);
        SetLanguage(language.SavedValue);
        SetMouseSensitivity(mouseSensitivity.SavedValue);

        masterVolumeSlider.value = masterVolume.SavedValue;
        musicVolumeSlider.value = musicVolume.SavedValue;
        sfxVolumeSlider.value = sfxVolume.SavedValue;
        mouseSensitivitySlider.value = mouseSensitivity.SavedValue;
    }
    private bool HasUnsavedChanges()
    {
        foreach(var setting in allSettings)
        {
            if(setting.Changed())
            {
                return true;
            }
        }
        return false;
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
            return;
        }
        optionsMenuCanvas.SetActive(false);
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
        saveChangesPrompt.SetActive(false);
        optionsMenuCanvas.SetActive(false);
    }
    public void Discard()
    {
        ApplySavedValues();
        saveChangesPrompt.SetActive(false);
        optionsMenuCanvas.SetActive(false);
    }
    public void Cancel()
    {
        saveChangesPrompt.SetActive(false);
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
    //---AUDIO SETTINGS---
    public void SetMasterVolume(float volume)
    {
        masterVolume.CurrentValue = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
        masterVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume.CurrentValue = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume.CurrentValue = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
        sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    //---CONTROLS SETTINGS---
    public void SetMouseSensitivity(float sensitivity)
    {
        mouseSensitivity.CurrentValue = (float)Math.Round(sensitivity, 2);
        mouseSensitivityText.text = mouseSensitivity.CurrentValue.ToString();
    }
    public void BeginRebindOperation()
    {

    }
    //---INTERNAL FUNCTIONS---
    private abstract class OptionSetting
    {
        public abstract void ApplyCurrentToSaved(); // As in: save new values
        public abstract void ApplySavedToCurrent(); // As in: revert changes
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
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
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
            return string.Equals(SavedValue, CurrentValue);
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
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
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
