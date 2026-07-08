using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel; // Reference to the main menu panel GameObject
    public GameObject creditsPanel; // Reference to the credits panel GameObject
    
    public void ContinueGame()
    {
        
    }
    public void StartNewGame()
    {
        //CHANGE THIS WHEN TUTORIAL LEVEL IS DONE
        UnityEngine.SceneManagement.SceneManager.LoadScene("TestArena");
    }
    public void OpenSettings()
    {
        // Open the settings menu
    }
    public void StartCreditsSequence()
    {
        // Start the credits sequence
        creditsPanel.SetActive(true);
    }
    public void ExitGame()
    {
        // Exit the game
        Debug.Log("This would exit the game");
        Application.Quit();
    }
}
