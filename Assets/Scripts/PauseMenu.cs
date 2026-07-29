using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : Singleton<PauseMenu>
{
    public GameObject pauseMenu;
    public GameObject backToMainMenuPrompt;
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
}
