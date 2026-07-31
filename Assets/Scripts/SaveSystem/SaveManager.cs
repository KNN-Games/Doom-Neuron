using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    //individual saves are divided to slots, each slot corresponds to a save file
    private string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");  // Path to the save folder
    //THERE ARE 6 SLOTS: 0,1,2,3,4,5

    //---SAVE GAME ON EVERY SCENE CHANGE---
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() //technically unnessary, but it is "good practice"
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) //save the game
    {
        if (scene.name == "MainMenu") return;
        Save(GameManager.Instance.saveSlot);
    }
    //------
    void Start()
    {
        Debug.Log($"Save folder path: {SaveFolder}");
        // Ensure the save folder exists (no need to check if it exists first, as CreateDirectory will do nothing if it already exists)
        Directory.CreateDirectory(SaveFolder);
    }
    public void Save()
    {
        Save(GameManager.Instance.saveSlot);
    }
    public void Save(int slot) //over-writes the save slot with new save
    {
        //Check if data is viable
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.LogError("SAVE FAILURE. CANNOT SAVE IN MAIN MENU");
            return;
        }
        if(slot < 0 || slot > 5)
        {
            Debug.LogError("SAVE FAILURE. INVALID SLOT: " + slot);
            return;
        }
        int diff = GameManager.Instance.difficulty;
        if(diff < 1 || diff > 3)
        {
            Debug.LogError("SAVE FAILURE. INVALID DIFFICULTY: " + diff);
            return;
        }
        //collect save data
        SaveData data = new()
        {
            difficulty = diff,
            playTime = GameManager.Instance.playTime,
            lastPlayed = System.DateTime.Now.Ticks,
            sceneName = SceneManager.GetActiveScene().name,

        };
        //save to a json file
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        File.WriteAllText(path, json);

        GameManager.Instance.lastSaved = 0;
        Debug.Log($"Game saved to {path} in slot {slot}");
    }
    public void LoadGame()
    {
        LoadGame(GameManager.Instance.saveSlot);
    }
    public void LoadGame(int slot)
    {
        SaveData data = Load(slot);
        if (data != null) //data found
        {
            // Load the game state from the save data
            GameManager.Instance.playTime = data.playTime;
            GameManager.Instance.difficulty = data.difficulty;
            GameManager.Instance.saveSlot = slot; //this part is technically not from data

            //DEBUG: print all save data info
            //Debug.Log(data.difficulty);
            //Debug.Log(data.playTime);
            //Debug.Log(data.lastPlayed);
            Debug.Log(data.sceneName);

            // Load the scene
            SceneManager.LoadScene(data.sceneName);
            Debug.Log($"Game loaded from slot {slot}");
        }
        else //start new game
        {
            MainMenu.Instance.OpenNewGameConfiguration(slot);
        }
    }
    public SaveData Load(int slot)
    {
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
    public void DeleteSave(int slot)
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
        MainMenu.Instance.UpdateSaveSlotUI();
    }
    public int GetSaveCount()
    {
        return Directory.GetFiles(SaveFolder, "save_*.json").Length;
    }
}
