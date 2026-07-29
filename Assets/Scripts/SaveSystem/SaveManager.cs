using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    //individual saves are divided to slots, each slot corresponds to a save file
    private string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");  // Path to the save folder
    //THERE ARE 6 SLOTS: 0,1,2,3,4,5
    
    void Start()
    {
        Debug.Log($"Save folder path: {SaveFolder}");
        // Ensure the save folder exists (no need to check if it exists first, as CreateDirectory will do nothing if it already exists)
        Directory.CreateDirectory(SaveFolder);
    }

    public void Save(int slot)
    {
        //collect save data
        SaveData data = new()
        {
            difficulty = GameManager.Instance.difficulty,
            playTime = GameManager.Instance.playTime,
            lastPlayed = System.DateTime.Now.Ticks,
            sceneName = SceneManager.GetActiveScene().name,

        };
        Debug.Log(System.DateTime.Now);
        //save to a json file
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        File.WriteAllText(path, json);
        Debug.Log($"Game saved to {path} in slot {slot}");
    }
    public void LoadGame(int slot)
    {
        SaveData data = Load(slot);
        if (data != null) //data found
        {
            // Load the game state from the save data
            GameManager.Instance.playTime = data.playTime;
            GameManager.Instance.difficulty = data.difficulty;

            //DEBUG: print all save data info
            Debug.Log(data.difficulty);
            Debug.Log(data.playTime);
            Debug.Log(data.lastPlayed);

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
