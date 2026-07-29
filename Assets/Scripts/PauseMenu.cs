using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : Singleton<PauseMenu>
{
    public GameObject pauseMenu;
    public GameObject backToMainMenuPrompt;
    public TextMeshProUGUI lastSavedText;
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public GameObject hud; //PlayerController.cs fills this reference when level starts
    public void OnOpenPauseMenu(InputAction.CallbackContext context)
    {
        if (!context.started || SceneManager.GetActiveScene().name == "MainMenu") return;

        if (!pauseMenu.activeSelf) //open menu
        {
            OpenPauseMenu();
        }
        else //close menu
        {
            ClosePauseMenu();
        }
    }
    public void OpenPauseMenu()
    {
        if (OptionsMenu.Instance.optionsMenuCanvas.activeSelf) return; //if options are active don't do anything
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        hud.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ClosePauseMenu()
    {
        if (OptionsMenu.Instance.optionsMenuCanvas.activeSelf) return; //if options are active don't do anything
        if(backToMainMenuPrompt.activeSelf)
        {
            backToMainMenuPrompt.SetActive(false);
        }
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        hud.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    //opening settings is in "OptionsMenu.cs"
    public void BackToCheckpoint()
    {
        //possibly unnessesary
    }
    public void OpenBackToMainMenuPrompt()
    {
        backToMainMenuPrompt.SetActive(true);
        lastSavedText.text = Format(GameManager.Instance.lastSaved);
    }
    public void ConfirmReturn()
    {
        ClosePauseMenu();
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
}
