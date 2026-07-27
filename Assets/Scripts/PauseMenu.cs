using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject hud;
    public GameObject backToMainMenuPrompt;
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
        if(OptionsMenu.Instance.optionsMenuCanvas.activeSelf) return; //if options are active don't do anything
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        hud.SetActive(false);
    }
    public void ClosePauseMenu()
    {
        if(OptionsMenu.Instance.optionsMenuCanvas.activeSelf) return; //if options are active don't do anything
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        hud.SetActive(true);
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
        SceneManager.LoadScene("MainMenu");
    }
    public void CancelReturn()
    {
        backToMainMenuPrompt.SetActive(false);
    }
}
