using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the player's UI: Pause menu, HUD, death screen, console.
/// </summary>
public class PlayerUI : Singleton<PlayerUI>
{
    [HideInInspector] public bool isPaused = false;
    [Header("References")]
    public GameObject pauseMenu; // CheatsManager.cs needs this to be public
    [SerializeField] private Button resumeButton;
    [SerializeField] private GameObject backToMainMenuPrompt;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject deathScreen;
    public void TogglePauseMenu()
    {
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
        if (OptionsMenu.Instance.IsOptionsMenuOpen) return; // If options are active don't do anything
        PauseGame();
        pauseMenu.SetActive(true);
        hud.SetActive(false);
        resumeButton.Select(); // Select resume button by default for non-mouse navigation
    }
    public void ClosePauseMenu()
    {
        if (OptionsMenu.Instance.IsOptionsMenuOpen) return; // If options are active don't do anything
        if (backToMainMenuPrompt.activeSelf)
        {
            backToMainMenuPrompt.SetActive(false);
        }
        UnpauseGame();
        pauseMenu.SetActive(false);
        hud.SetActive(true);
    }
    public void OpenSettings() // Activated via Canvas button
    {
        OptionsMenu.Instance.OpenOptionsMenu();
    }
    //---DIRECT PAUSE---
    public void PauseGame()
    { 
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        hud.SetActive(false);
        InputManager.Instance.SetActiveInputMap("UI");
        Debug.Log("Paused Game");
    }
    public void UnpauseGame()
    {
        if(PlayerHealth.Instance.IsDead) return; // Do not allow unpause if player is dead
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hud.SetActive(true);
        InputManager.Instance.SetActiveInputMap("Gameplay");
        Debug.Log("Unpaused Game");
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
    public void BackToCheckpoint() // Activated via Canvas button
    {
        PlayerHealth.Instance.Resurrect();
        SaveManager.Instance.LoadGame();
    }
    public void ShowDeathScreen()
    {
        PauseGame();
        deathScreen.SetActive(true);
    }
    public void HideDeathScreen()
    {
        UnpauseGame();
        deathScreen.SetActive(false);
    }
}
