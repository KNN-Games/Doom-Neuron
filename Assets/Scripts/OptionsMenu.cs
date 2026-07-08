using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //SUMMARY:
    //when u change a slider/button/whatever u should feel the effects immediatly, but not save them until u hit the save button.
    //when adding a new setting u have to add:
    //1. a variable to hold the value of the setting
    //2. a function to set the value of the setting 
    //3. modify Start() to load the setting from PlayerPrefs and set the variable
    //4. modify BackToMenu() to check if the setting has changed
    //5. modify SaveChanges() to & DiscardChanges() to save/discard the setting
    //6. modify ResetSettings() to reset the setting to default value
    public AudioMixer audioMixer;
    [Header("UI References")]
    //the whole canvas
    public GameObject optionsMenuCanvas;
    //panels
    public GameObject generalSettingsPanel;
    public GameObject audioSettingsPanel;
    public GameObject controlsSettingsPanel;
    public GameObject saveChangesPrompt;
    [Header("Audio UI")]
    //slider texts
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    //sliders
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    //variables to hold the settings values
    private float savedMasterVolume;
    private float savedMusicVolume;
    private float savedsfxVolume;
    private string savedLanguage;

    public void Start()
    {
        //load variables
        //check if settings exist and load defaults if not
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            SetMasterVolume(PlayerPrefs.GetFloat("masterVolume"));
        }
        else
        {
            SetMasterVolume(1f);
            PlayerPrefs.SetFloat("masterVolume", 1f);
            Debug.Log("Default master volume loaded");
        }
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        }
        else
        {
            SetMusicVolume(1f);
            PlayerPrefs.SetFloat("musicVolume", 1f);
            Debug.Log("Default music volume loaded");
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        }
        else
        {
            SetSFXVolume(1f);
            PlayerPrefs.SetFloat("sfxVolume", 1f);
            Debug.Log("Default sfx volume loaded");
        }
        if (PlayerPrefs.HasKey("language"))
        {
            SetLanguage(PlayerPrefs.GetString("language"));
        }
        else
        {
            SetLanguage("en");
            PlayerPrefs.SetString("language", "en");
            Debug.Log("Default language loaded");
        }

        Debug.Log("Settings loaded. Values: ");
        Debug.Log("Master Volume: " + PlayerPrefs.GetFloat("masterVolume") + ", Music Volume: " + PlayerPrefs.GetFloat("musicVolume") + ", SFX Volume: " + PlayerPrefs.GetFloat("sfxVolume"));

        //save variables from playersPrefs to local variables
        savedMasterVolume = PlayerPrefs.GetFloat("masterVolume");
        savedMusicVolume = PlayerPrefs.GetFloat("musicVolume");
        savedsfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        savedLanguage = PlayerPrefs.GetString("language");
    }
    public void OpenOptionsMenu()
    {
        optionsMenuCanvas.SetActive(true);
        OpenGeneralSettings();
        //load slider values
        masterVolumeSlider.value = savedMasterVolume; //saved should have the same value as playerPrefs (the real value), right? 
        musicVolumeSlider.value = savedMusicVolume; //At least when the options menu is opened.
        sfxVolumeSlider.value = savedsfxVolume;
    }
    public void SaveChanges()
    {
        //save to playerPrefs
        PlayerPrefs.SetFloat("masterVolume", savedMasterVolume);
        PlayerPrefs.SetFloat("musicVolume", savedMusicVolume);
        PlayerPrefs.SetFloat("sfxVolume", savedsfxVolume);
        PlayerPrefs.SetString("language", savedLanguage);

        PlayerPrefs.Save();
    }
    private void DiscardChanges()
    {
        //revert to saved values
        SetMasterVolume(PlayerPrefs.GetFloat("masterVolume"));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        SetLanguage(PlayerPrefs.GetString("language"));
    }

    //---Left bar buttons---
    public void BackToMenu()
    {
        bool changesMade = false;

        if (PlayerPrefs.GetFloat("masterVolume") != savedMasterVolume) changesMade = true;
        if (PlayerPrefs.GetFloat("musicVolume") != savedMusicVolume) changesMade = true;
        if (PlayerPrefs.GetFloat("sfxVolume") != savedsfxVolume) changesMade = true;
        if (PlayerPrefs.GetString("language") != savedLanguage) changesMade = true;
        //check if any settings have changed
        if (changesMade)
        {
            //open "are you sure" prompt. It gives 3 options: confirm, discard, cancel.
            //it blocks u from clicking any other button because of a background with raycast target enabled.
            saveChangesPrompt.SetActive(true);
        }
        else
        {
            SaveChanges();
            optionsMenuCanvas.SetActive(false);
        }
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
        SetMasterVolume(1f);
        SetMusicVolume(1f);
        SetSFXVolume(1f);
        SetLanguage("en");
        BackToMenu();
    }
    //---Save changes prompt buttons---
    public void Confirm()
    {
        SaveChanges();
        saveChangesPrompt.SetActive(false);
        optionsMenuCanvas.SetActive(false);
    }
    public void Discard()
    {
        DiscardChanges();
        saveChangesPrompt.SetActive(false);
        optionsMenuCanvas.SetActive(false);
    }
    public void Cancel()
    {
        saveChangesPrompt.SetActive(false);
    }

    //---General settings---
    public void SetLanguage(string languageCode)
    {
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
    //---Audio settings---
    public void SetMasterVolume(float volume)
    {
        savedMasterVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MasterVolume", dB);
        masterVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetMusicVolume(float volume)
    {
        savedMusicVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
        musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    public void SetSFXVolume(float volume)
    {
        savedsfxVolume = volume;
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat("SoundEffectsVolume", dB);
        sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
    }
    //---Controls settings---
    public void SetButtonToAction()
    {
        //this one will be hard to do.
    }
}
