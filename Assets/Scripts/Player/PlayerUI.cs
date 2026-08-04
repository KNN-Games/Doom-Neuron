using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the player's UI: Pause menu, HUD.
/// </summary>
public class PlayerUI : Singleton<PlayerUI>
{
    [HideInInspector] public bool isPaused = false;
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject backToMainMenuPrompt;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject deathScreen;
    public void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.started || SceneManager.GetActiveScene().name == "MainMenu" || CheatsManager.Instance.IsConsoleActive) return;

        if (!isPaused) //open menu
        {
            OpenPauseMenu();
        }
        else //close menu
        {
            ClosePauseMenu();
        }
    }
    //---PAUSE MENU---
    public void OpenPauseMenu()
    {
        if (OptionsMenu.Instance.IsOptionsMenuOpen) return; //if options are active don't do anything
        PauseGame();
        pauseMenu.SetActive(true);
        hud.SetActive(false);
    }
    public void ClosePauseMenu()
    {
        if (OptionsMenu.Instance.IsOptionsMenuOpen) return; //if options are active don't do anything
        if (backToMainMenuPrompt.activeSelf)
        {
            backToMainMenuPrompt.SetActive(false);
        }
        UnpauseGame();
        pauseMenu.SetActive(false);
        hud.SetActive(true);
    }
    public void OpenSettings()
    {
        OptionsMenu.Instance.OpenOptionsMenu();
    }
    public void BackToCheckpoint()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.LoadGame();
    }
    //---DIRECT PAUSE---
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        hud.SetActive(false);
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hud.SetActive(true);
    }
    //---BACK TO MENU PROMPT---
    public void OpenBackToMainMenuPrompt()
    {
        backToMainMenuPrompt.SetActive(true);
        lastSavedText.text = Format(GameManager.Instance.lastSaved);
    }
    public void ConfirmReturn()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void CancelReturn()
    {
        backToMainMenuPrompt.SetActive(false);
    }
    private static string Format(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return string.Format
        (
            "{0:D3}:{1:D2}:{2:D2}",
            (int)time.TotalHours,
            time.Minutes,
            time.Seconds
        );
    }
    //---DEATH SCREEN---
    public void ShowDeathScreen()
    {
        PauseGame();
        deathScreen.SetActive(true);
    }
}
