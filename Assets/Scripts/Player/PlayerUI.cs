using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles the player's UI: Pause menu, HUD, death screen, console.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerUI : Singleton<PlayerUI>
{
    [HideInInspector] public bool isPaused = false;
    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private GameObject backToMainMenuPrompt;
    [SerializeField] private TextMeshProUGUI lastSavedText;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject deathScreen;
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindActionMap("Gameplay").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
        // Check if there is only 1 PlayerInput in scene. Some stuff in options menu depend on it.
        int playerInputCount = PlayerInput.all.Count;
        if(playerInputCount != 1)
        {
            Debug.LogError("DETECTED MORE THAN ONE PLAYER INPUT COMPONENT IN SCENE. ONLY ONE SHOULD EXIST. CURRENT NUMBER: " + playerInputCount);
        }
    }
    // Handle input actions
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
    // Handle console-related inputs
    public void OnOpenConsole(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        CheatsManager.Instance.ToggleConsole();
    }
    public void OnSubmitCommand(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        CheatsManager.Instance.SubmitCommand();
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
    public void BackToCheckpoint() // Activated via Canvas button
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
        // Force Global + UI to be enabled. New Input System kinda sucks in this regard. I can't use switchSwitchCurrentActionMap cuz it would disable global map
        playerInput.actions.FindActionMap("Gameplay").Disable();
        playerInput.actions.FindActionMap("UI").Enable();
        Debug.Log("Paused Game");
    }
    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hud.SetActive(true);
        playerInput.actions.FindActionMap("Gameplay").Enable();
        playerInput.actions.FindActionMap("UI").Disable();
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
    public void ShowDeathScreen()
    {
        PauseGame();
        deathScreen.SetActive(true);
    }
}
