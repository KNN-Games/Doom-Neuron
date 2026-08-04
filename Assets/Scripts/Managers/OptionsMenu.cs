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

    [Header("Audio UI References")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    // Persisted values: what is already saved to disk (used to discard changes and check for unsaved changes).
    private float savedMasterVolume;
    private float savedMusicVolume;
    private float savedSfxVolume;
    private string savedLanguage;

    // Current values: live preview state while the options menu is open.
    private float currentMasterVolume;
    private float currentMusicVolume;
    private float currentSfxVolume;
    private string currentLanguage;

    private void Start()
    {
        // Load the saved values from PlayerPrefs, or set to default if not found.
        // Master volume
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            savedMasterVolume = PlayerPrefs.GetFloat("masterVolume");
        }
        else
        {
            savedMasterVolume = 1f;
            PlayerPrefs.SetFloat("masterVolume", savedMasterVolume);
        }
        // Music volume
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            savedMusicVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        else
        {
            savedMusicVolume = 1f;
            PlayerPrefs.SetFloat("musicVolume", savedMusicVolume);
        }
        // SFX volume
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            savedSfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            savedSfxVolume = 1f;
            PlayerPrefs.SetFloat("sfxVolume", savedSfxVolume);
        }
        // Language
        if (PlayerPrefs.HasKey("language"))
        {
            savedLanguage = PlayerPrefs.GetString("language");
        }
        else
        {
            savedLanguage = "en";
            PlayerPrefs.SetString("language", savedLanguage);
        }

        // Set current values as the saved values initially, so that the UI reflects the saved state when the menu is opened.
        ApplySavedValues();

        // Apply the saved values to the actual settings.
        PlayerPrefs.Save();

        // DEBUG: print all saved values
        Debug.Log(
        $"Settings loaded. Values:\n" +
        $"Master Volume: {savedMasterVolume}\n" +
        $"Music Volume: {savedMusicVolume}\n" +
        $"SFX Volume: {savedSfxVolume}\n" +
        $"Language: {savedLanguage}");
    }
    private void ApplySavedValues() // Update settings & UI to match SAVED values.
    {
        // Set CURRENT values to match SAVED values.
        SetMasterVolume(savedMasterVolume);
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSfxVolume);
        SetLanguage(savedLanguage);

        // Update the sliders to reflect the current values.
        masterVolumeSlider.value = savedMasterVolume;
        musicVolumeSlider.value = savedMusicVolume;
        sfxVolumeSlider.value = savedSfxVolume;
    }
    private bool HasUnsavedChanges()
    {
        return !Mathf.Approximately(currentMasterVolume, savedMasterVolume)
            || !Mathf.Approximately(currentMusicVolume, savedMusicVolume)
            || !Mathf.Approximately(currentSfxVolume, savedSfxVolume)
            || currentLanguage != savedLanguage;
    }
    public void SaveChanges() // Used then player confirms changes in prompt OR when player hits save button in options menu
    {
        // Change SAVED to match CURRENT
        savedMasterVolume = currentMasterVolume;
        savedMusicVolume = currentMusicVolume;
        savedSfxVolume = currentSfxVolume;
        savedLanguage = currentLanguage;

        // Persist the SAVED values to PlayerPrefs
        PlayerPrefs.SetFloat("masterVolume", savedMasterVolume);
        PlayerPrefs.SetFloat("musicVolume", savedMusicVolume);
        PlayerPrefs.SetFloat("sfxVolume", savedSfxVolume);
        PlayerPrefs.SetString("language", savedLanguage);
        PlayerPrefs.Save();
    }
    //---MENU NAVIGATION---
    public void OpenOptionsMenu() // Used by PlayerUI and MainMenu to open the options menu.
    {
        optionsMenuCanvas.SetActive(true);
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
        savedMasterVolume = 1f;
        savedMusicVolume = 1f;
        savedSfxVolume = 1f;
        savedLanguage = "en";

        ApplySavedValues();
        SaveChanges(); // Only the PlayerPrefs part of this function matters.
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
        currentLanguage = languageCode;
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
        currentMasterVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
        masterVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetSFXVolume(float volume)
    {
        currentSfxVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
        sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    //---CONTROLS SETTINGS---
    public void SetButtonToAction()
    {
        //this one will be hard to do.
    }
}
