using System.Collections;
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
    [SerializeField] private SplashScreen splashScreen;
    [SerializeField] private Button[] mainMenuButtons;
    [SerializeField] private Button[] difficultyButtons; // 0 - easy, 1 - medium, 2 - hard
    [Header("Settings")]
    [SerializeField] private float fadeInDuration;
    [HideInInspector] public bool isInSplashScreen;
    private SaveManager saveManager;
    private int selectedDifficulty = 2; // Track the selected difficulty level, 2 (medium) is default

    protected override void Awake()
    {
        base.Awake();
        // Destroy player if it exists, because the player is not supposed to exist in the main menu
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
    private void Start() // Technically more efficent than calling instance every time
    {
        saveManager = SaveManager.Instance;
    }
    //---MAIN SCREEN---
    public void OpenSlotSelection()
    {
        // If 0 save files exist start new game immediately for dramatic effect.
        if (SaveManager.Instance.GetSaveCount() == 0)
        {
            SaveManager.Instance.saveSlot = 0;
            OpenNewGameConfiguration();
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
        SaveData data = saveManager.Load(slot);
        saveManager.saveSlot = slot; // VERY IMPORTANT! This is the only place where saveSlot is set
        if (data == null)
        {
            OpenNewGameConfiguration();
            return;
        }
        saveManager.LoadGame(data);
    }
    public void DeleteSave(int slot)
    {
        saveManager.DeleteSave(slot);
        UpdateSaveSlotUI();
    }
    //---NEW GAME CONFIG SCREEN---
    public void ConfirmGame() // Start new game
    {
        SaveData newGameData = new()
        {
            saveSlot = saveManager.saveSlot,
            difficulty = selectedDifficulty,
            playTime = 0f,
            lastPlayed = System.DateTime.Now.Ticks,
            sceneName = "TestArena",
            playerPosition = Vector3.zero,
            playerRotation = Vector3.zero
        };
        saveManager.LoadGame(newGameData);
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
    //---HELPER METHODS---
    public void EndSplashScreen() // Used by SplashScreen.cs
    {
        if (!isInSplashScreen)return;
        isInSplashScreen = false;
        splashScreen.gameObject.SetActive(false);
        StartCoroutine(FadeInMainMenu(fadeInDuration));
    }
    private void OpenMainMenu()
    {
        mainMenuPanel.SetActive(true);
        saveSlotPanel.SetActive(false);
        //Select "Start" button by default for non-mouse navigation
        mainMenuButtons[0].Select();
    }
    private IEnumerator FadeInMainMenu(float duration)
    {
        //Activate
        mainMenuPanel.SetActive(true);
        foreach(Button button in mainMenuButtons)
        {
            button.interactable = false;
        }
        CanvasGroup canvasGroup = mainMenuPanel.GetComponent<CanvasGroup>();
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        foreach(Button button in mainMenuButtons)
        {
            button.interactable = true;
        }
    }
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
    private void OpenNewGameConfiguration() // No button does this directly, only based on context.
    {
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
            SaveData data = saveManager.Load(i);
            saveSlots[i].GetComponent<SlotTextHandler>().UpdateSlot(data);
        }
    }
}