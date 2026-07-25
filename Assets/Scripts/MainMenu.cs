using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public GameObject mainMenuPanel; // Reference to the main menu panel GameObject
    public GameObject creditsPanel; // Reference to the credits panel GameObject
    public GameObject saveSlotPanel;
    public GameObject newGameConfigPanel;
    public Button[] difficultyButtons; // Array of difficulty buttons
    public GameObject[] noSaveText;
    public GameObject[] yesSaveText;
    public LocalizeStringEvent[] difficultyText;
    private int selectedSlot = -1; // Track the selected save slot
    private int selectedDifficulty = 2; // Track the selected difficulty level (default to 1)

    //MAIN MENU -> SLOT SELECTION -> GAME CONGIFURATION -> INTRO -> GAME
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of MainMenu detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    public void StartGame() //open slot selection menu
    {
        //if 0 save files exist start new game immediately for dramatic effect.
        if(SaveManager.Instance.GetSaveCount() == 0)
        {
            OpenNewGameConfiguration(0);
        }
        else
        {
            saveSlotPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            for(int i = 0; i <= 5; i++) //check if save exists
            {
                SaveData data = SaveManager.Instance.Load(i);
                if(data != null)
                {
                    yesSaveText[i].SetActive(true);
                    noSaveText[i].SetActive(false);
                    // Update the difficulty text based on the loaded save data
                    switch (data.difficulty)
                    {
                        case 1:
                            difficultyText[i].StringReference.TableEntryReference = "EASY DIFFICULTY";
                            break;
                        case 2:
                            difficultyText[i].StringReference.TableEntryReference = "MEDIUM DIFFICULTY";
                            break;
                        case 3:
                            difficultyText[i].StringReference.TableEntryReference = "HARD DIFFICULTY";
                            break;
                        default:
                            Debug.LogWarning($"Unknown difficulty level {data.difficulty} in save slot {i}. DELETING FAULTY SAVE");
                            SaveManager.Instance.DeleteSave(i);
                            break;
                    }
                    //Update time played text

                    //Update last played text

                } else
                {
                    noSaveText[i].SetActive(true);
                    yesSaveText[i].SetActive(false);
                }
            }
        }
    }
    //loading game is in "SaveManager.cs"
    //opening settings is in "OptionsMenu.cs"
    public void StartCreditsSequence()
    {
        // Start the credits sequence
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);


    }
    public void ExitGame()
    {
        // Exit the game
        Debug.Log("This would exit the game");
        Application.Quit();
    }
    //--- New Game configuration functions ---
    public void OpenNewGameConfiguration(int slot)
    {
        selectedSlot = slot;
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        newGameConfigPanel.SetActive(true);
    }
    public void ConfirmGame()
    {
        // Reset game data
        GameManager.Instance.playTime = 0f;
        GameManager.Instance.difficulty = selectedDifficulty;

        // Initialize new game data
        SaveManager.Instance.Save(selectedSlot);
        SceneManager.LoadScene("TestArena");
    }
    public void SetNewGameDifficulty(int difficulty)
    {
        for(int i = 1; i >= 3; i++)
        {
            if(i == difficulty)
            {
                difficultyButtons[i].image.color = Color.red;
            } else
            {
                difficultyButtons[i].image.color = Color.white;
            }
        }
        selectedDifficulty = difficulty;
        GameManager.Instance.difficulty = difficulty;
    }
}
