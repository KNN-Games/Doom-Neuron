using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu>
{
    //MAIN MENU -> SLOT SELECTION -> GAME CONGIFURATION -> (if new game)INTRO -> GAME
    //loading game is in "SaveManager.cs"
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject saveSlotPanel;
    public GameObject newGameConfigPanel;
    public Button[] difficultyButtons; // 0 - easy, 1 - medium, 2 - hard
    public GameObject[] saveSlots;
    private int selectedSlot = -1; // Track the selected save slot, -1 means none
    private int selectedDifficulty = 2; // Track the selected difficulty level, 2 (medium) is default

    //---MAIN SCREEN---
    public void OpenSlotSelection()
    {
        //if 0 save files exist start new game immediately for dramatic effect.
        if (SaveManager.Instance.GetSaveCount() == 0)
        {
            OpenNewGameConfiguration(0);
        }
        else
        {
            saveSlotPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            UpdateSaveSlotUI();
        }
    }
    public void OpenSettings()
    {
        OptionsMenu.Instance.OpenOptionsMenu();
    }
    public void StartCreditsSequence()
    {
        // Start the credits sequence
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);


    }
    public void ExitGame() // Exit the game
    {
        Debug.Log("This would exit the game");
        Application.Quit();
    }
    //--=SLOT SELECT SCREEN--=
    public void UpdateSaveSlotUI() //update every slot
    {
        for (int i = 0; i <= 5; i++)
        {
            SaveData data = SaveManager.Instance.Load(i);
            saveSlots[i].GetComponent<SlotTextHandler>().UpdateSlot(data);
        }
    }
    public void LoadGame(int slot)
    {
        SaveManager.Instance.LoadGame(slot);
    }
    //---NEW GAME CONFIG SCREEN---
    public void OpenNewGameConfiguration(int slot)
    {
        selectedSlot = slot;
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        newGameConfigPanel.SetActive(true);
        difficultyButtons[1].image.color = Color.red; //highlight medium difficulty button, because it's the default difficulty
        selectedDifficulty = 2;
    }
    public void ConfirmGame() //play intro (not implemented yet)
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
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            if (i + 1 == difficulty)
            {
                difficultyButtons[i].image.color = Color.red;
            }
            else
            {
                difficultyButtons[i].image.color = Color.white;
            }
        }
        selectedDifficulty = difficulty;
        GameManager.Instance.difficulty = difficulty;
    }
    //---OTHER---
    public void ReturnToMainMenu() //move 1 step backwards
    {
        if(newGameConfigPanel.activeSelf)
        {
            //move back to slots
            newGameConfigPanel.SetActive(false);
            OpenSlotSelection();
        } else //assume saveSlotPanel is active
        {
            mainMenuPanel.SetActive(true);
            saveSlotPanel.SetActive(false);
        }
    }
}