using UnityEngine;

/// <summary>
/// This file contains the navigation logic for the OptionsMenu.
/// </summary>
public partial class OptionsMenu
{
    /// <summary>
    /// Open the options menu. Used by PlayerUI.cs and MainMenu.cs to open the options menu.
    /// </summary>
    public void OpenOptionsMenu()
    {
        optionsMenuCanvas.SetActive(true);
        generalSettingsButton.Select();
        OpenSettingsPanel(generalSettingsPanel);

        // Reset preview state to the last persisted values whenever the menu opens.
        ApplySavedValues();
    }
    /// <summary>
    /// Used by the back button in the options menu. Checks for unsaved changes.
    /// </summary>
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
    /// <summary>
    /// Opens the specified settings panel and closes the others.
    /// </summary>
    /// <param name="panelToOpen">The settings panel to open.</param>
    public void OpenSettingsPanel(GameObject panelToOpen)
    {
        generalSettingsPanel.SetActive(panelToOpen == generalSettingsPanel);
        graphicsSettingsPanel.SetActive(panelToOpen == graphicsSettingsPanel);
        if (panelToOpen == graphicsSettingsPanel)
        {
            UpdateAvailableResolutions();
        }
        audioSettingsPanel.SetActive(panelToOpen == audioSettingsPanel);
        controlsSettingsPanel.SetActive(panelToOpen == controlsSettingsPanel);
        if (panelToOpen == controlsSettingsPanel)
        {
            UpdateKeyRebindButtons();
        }
    }
    /// <summary>
    /// Used by the "Reset to Default" button in the options menu.
    /// </summary>
    public void ResetSettings()
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
    /// <summary>
    /// Used by the "Reset specific setting" button in the options menu. (rn only used for sliders)
    /// </summary>
    /// <param name="settingName">The name of the setting to reset.</param>
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
    /// <summary>
    /// Used by the "Confirm" button in the "Confirm Changes" prompt.
    /// </summary>
    public void Confirm()
    {
        SaveChanges();
        ExitSettings();
    }
    /// <summary>
    /// Used by the "Discard" button in the "Confirm Changes" prompt.
    /// </summary>
    public void Discard()
    {
        ApplySavedValues();
        ExitSettings();
    }
    /// <summary>
    /// Used by the "Cancel" button in the "Confirm Changes" prompt.
    /// </summary>
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
}