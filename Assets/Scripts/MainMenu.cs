using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private Button[] difficultyButtons; // 0 - easy, 1 - medium, 2 - hard
    [SerializeField] private GameObject[] saveSlots;
    private int selectedSlot = -1; // Track the selected save slot, -1 means none
    private int selectedDifficulty = 2; // Track the selected difficulty level, 2 (medium) is default

    protected override void Awake()
    {
        base.Awake();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) // Destroy player if it exists, because the player is not supposed to exist in the main menu
        {
            Destroy(GameObject.FindWithTag("Player"));
        }
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
    //---SLOT SELECT SCREEN---
    private void UpdateSaveSlotUI() // Update every slot
    {
        for (int i = 0; i <= 5; i++)
        {
            SaveData data = SaveManager.Instance.Load(i);
            saveSlots[i].GetComponent<SlotTextHandler>().UpdateSlot(data);
        }
    }
    public void LoadGame(int slot)
    {
        GameManager.Instance.saveSlot = slot;
        SaveManager.Instance.LoadGame(slot);
    }
    public void DeleteSave(int slot)
    {
        SaveManager.Instance.DeleteSave(slot);
        UpdateSaveSlotUI();
    }
    //---NEW GAME CONFIG SCREEN---
    public void OpenNewGameConfiguration(int slot)
    {
        selectedSlot = slot;
        mainMenuPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
        newGameConfigPanel.SetActive(true);
        difficultyButtons[1].image.color = Color.red; // Highlight medium difficulty button, because it's the default difficulty
        selectedDifficulty = 2;
    }
    public void ConfirmGame() //play intro (not implemented yet)
    {
        // Later on make it so player is created in the intro scene, but for now just create player in the first level.
        // Reset game data
        GameManager.Instance.playTime = 0f;
        GameManager.Instance.difficulty = selectedDifficulty;

        // Initialize new game data
        SaveManager.Instance.Save(selectedSlot);

        SceneManager.LoadScene("TestArena");
        //CHANGE THIS LATER!!!
        const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        GameObject player = Instantiate(playerPrefab);
        player.name = "Player";
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
    public void ReturnToMainMenu() // Move 1 step backwards
    {
        if(newGameConfigPanel.activeSelf)
        {
            // Move back to slots
            newGameConfigPanel.SetActive(false);
            OpenSlotSelection();
        } else // Assume saveSlotPanel is active
        {
            mainMenuPanel.SetActive(true);
            saveSlotPanel.SetActive(false);
        }
    }
}