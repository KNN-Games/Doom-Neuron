using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles saving, loading and deleting save files.
/// Auto-saves on every viable scene change.
/// Individual saves are divided to slots, each slot corresponds to a save file.
/// There are 6 slots: 0,1,2,3,4,5
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    private string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");  // Path to the save folder
    private const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab"; // Path to the player prefab

    //---SAVE GAME ON EVERY SCENE CHANGE---
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy() // Technically unnessary, but it is "good practice"
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // Save the game
    {
        if (scene.name == "MainMenu") return;
        Save(GameManager.Instance.saveSlot);
    }
    //------
    private void Start()
    {
        //Debug.Log($"Save folder path: {SaveFolder}");

        // Ensure the save folder exists (no need to check if it exists first, as CreateDirectory will do nothing if it already exists)
        Directory.CreateDirectory(SaveFolder);
    }
    //---SAVING---
    public void Save()
    {
        Save(GameManager.Instance.saveSlot);
    }
    public void Save(int slot) // Over-writes the save slot with new save
    {
        // Check if data is viable
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.LogError("SAVE FAILURE. CANNOT SAVE IN MAIN MENU");
            return;
        }
        if (slot < 0 || slot > 5)
        {
            Debug.LogError("SAVE FAILURE. INVALID SLOT: " + slot);
            return;
        }
        int diff = GameManager.Instance.difficulty;
        if (diff < 1 || diff > 3)
        {
            Debug.LogError("SAVE FAILURE. INVALID DIFFICULTY: " + diff);
            return;
        }

        // Create save data object
        SaveData data = new()
        {
            saveSlot = slot,
            difficulty = diff,
            playTime = GameManager.Instance.playTime,
            lastPlayed = System.DateTime.Now.Ticks,
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = PlayerController.Instance.transform.position,
            playerRotation = PlayerController.Instance.transform.rotation.eulerAngles
        };

        // Save to a json file
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        File.WriteAllText(path, json);

        GameManager.Instance.lastSaved = 0;
        Debug.Log($"Game saved to {path} in slot {slot}");
    }
    //---LOADING---
    public SaveData Load(int slot) // Returns the save data from the specified slot, or null if no save data exists for that slot
    {
        if (slot < 0 || slot > 5)
        {
            Debug.LogError("LOAD FAILURE. INVALID SLOT: " + slot);
            return null;
        }
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return null;
        }
    }
    public void LoadGame()
    {
        LoadGame(Load(GameManager.Instance.saveSlot));
    }
    public void LoadGame(SaveData data) // I don't know if I should move this to GameManager.
    {
        // Load the game state from the save data
        SceneManager.LoadScene(data.sceneName);
        CreatePlayer();
        PlayerController.Instance.transform.position = data.playerPosition;
        PlayerController.Instance.transform.localEulerAngles = data.playerRotation;
        GameManager.Instance.playTime = data.playTime;
        GameManager.Instance.difficulty = data.difficulty;
        GameManager.Instance.saveSlot = data.saveSlot;

        // DEBUG: print all save data info
        Debug.Log(
        $"Game loaded. Values:\n" +
        $"save slot: {data.saveSlot}\n" +
        $"difficulty: {data.difficulty}\n" +
        $"playTime: {data.playTime}\n" +
        $"lastPlayed: {data.lastPlayed}\n" +
        $"sceneName: {data.sceneName}\n");
    }
    //------
    public void DeleteSave(int slot) // Used in main menu slot selection screen via mainMenu.cs
    {
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogWarning($"Save file not found at {path}");
        }
    }
    public int GetSaveCount()
    {
        return Directory.GetFiles(SaveFolder, "save_*.json").Length;
    }
    private void CreatePlayer() // Create player in the scene, if it doesn't already exist
    {
        if (PlayerController.Instance != null) return;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found in: " + PlayerPrefabPath);
            return;
        }
        GameObject player = Instantiate(prefab);
        player.name = "Player"; // Set the name of the instantiated player object
        InputManager.Instance.SetPlayer(player);
    }
}
