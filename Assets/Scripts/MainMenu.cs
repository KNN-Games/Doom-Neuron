using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu, including save slot selection and new game configuration.
/// </summary>
public class MainMenu : Singleton<MainMenu>
{
    // MAIN MENU -> SLOT SELECTION -> GAME CONGIFURATION -> (if new game)INTRO -> GAME
    [Header("References")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject saveSlotPanel;
    [SerializeField] private GameObject newGameConfigPanel;
    [SerializeField] private GameObject[] saveSlots;
    [SerializeField] private Button startButton;
    [SerializeField] private Button[] difficultyButtons; // 0 - easy, 1 - medium, 2 - hard
    private int selectedSlot = -1; // Track the selected save slot, -1 means none
    private int selectedDifficulty = 2; // Track the selected difficulty level, 2 (medium) is default

    protected override void Awake()
    {
        base.Awake();
        //Select "Start" button by default for non-mouse navigation
        startButton.Select();

        // Destroy player if it exists, because the player is not supposed to exist in the main menu
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
    public void OpenMainMenu() // Used by SplashScreen.cs
    {
        mainMenuPanel.SetActive(true);
        saveSlotPanel.SetActive(false);
        startButton.Select();
    }
    //---MAIN SCREEN---
    public void OpenSlotSelection()
    {
        // If 0 save files exist start new game immediately for dramatic effect.
        if (SaveManager.Instance.GetSaveCount() == 0)
        {
            OpenNewGameConfiguration(0);
        }
        else
        {
            saveSlots[0].GetComponent<Button>().Select(); // Select first slot by default for non-mouse navigation
            saveSlotPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            UpdateSaveSlotUI();
        }
    }
    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        OptionsMenu.Instance.OpenOptionsMenu();
    }
    public void StartCreditsSequence() //Tutaj, Maciej Fedorowicz. W tym miejscu dodaj kod do rozpoczęcia sekwencji napisów końcowych.
    {
        // Start the credits sequence
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);


    }
    public void ExitGame()
    {
        Debug.Log("This would exit the game");
        Application.Quit();
    }
    //---SLOT SELECT SCREEN---
    public void LoadGame(int slot)
    {
        SaveData data = SaveManager.Instance.Load(slot);
        if (data == null)
        {
            OpenNewGameConfiguration(slot);
            return;
        }
        GameManager.Instance.saveSlot = slot;
        SaveManager.Instance.LoadGame(data);
    }
    public void DeleteSave(int slot)
    {
        SaveManager.Instance.DeleteSave(slot);
        UpdateSaveSlotUI();
    }
    //---NEW GAME CONFIG SCREEN---
    public void ConfirmGame() // Start new game
    {
        SaveData newGameData = new()
        {
            saveSlot = selectedSlot,
            difficulty = selectedDifficulty,
            playTime = 0f,
            lastPlayed = System.DateTime.Now.Ticks,
            sceneName = "TestArena",
            playerPosition = Vector3.zero,
            playerRotation = Vector3.zero
        };
        SaveManager.Instance.LoadGame(newGameData);
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
    public void ReturnToMainMenu() // Used by buttons. Move 1 step backward.
    {
        if (newGameConfigPanel.activeSelf)
        {
            // Move back to slots
            newGameConfigPanel.SetActive(false);
            OpenSlotSelection();
        }
        else // Assume saveSlotPanel is active or going to from options menu - ExitSettings() in OptionsMenu.cs
        {
            OpenMainMenu();
        }
    }
    //---HELPER METHODS---
    private void OpenNewGameConfiguration(int slot) // No button does this directly, only based on context.
    {
        selectedSlot = slot;
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        newGameConfigPanel.SetActive(true);
        // Select medium difficulty by default
        difficultyButtons[1].Select();
        SetNewGameDifficulty(2);
    }
    private void UpdateSaveSlotUI() // Update every slot
    {
        for (int i = 0; i <= 5; i++)
        {
            SaveData data = SaveManager.Instance.Load(i);
            saveSlots[i].GetComponent<SlotTextHandler>().UpdateSlot(data);
        }
    }
}