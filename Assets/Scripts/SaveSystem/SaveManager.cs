using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //individual saves are divided to slots, each slot corresponds to a save file
    public static SaveManager Instance;
    private string SaveFolder => Path.Combine(Application.persistentDataPath, "Saves");  // Path to the save folder
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of SaveManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
        Debug.Log($"Save folder path: {SaveFolder}");

        // Ensure the save folder exists (no need to check if it exists first, as CreateDirectory will do nothing if it already exists)
        Directory.CreateDirectory(SaveFolder);
        // Optionally, you can log the number of save files found
        int saveCount = GetSaveCount();
        Debug.Log($"Number of save files found: {saveCount}");
    }

    public void Save(int slot, SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(SaveFolder, $"save_{slot}.json");
        File.WriteAllText(path, json);
        Debug.Log($"Game saved to {path} in slot {slot}");
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
            Debug.LogWarning($"Save file not found at {path}");
            return null;
        }
    }
    public int GetSaveCount()
    {
        return Directory.GetFiles(SaveFolder, "save_*.json").Length;
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
    }
    public void StartNewGame()
    {
        SaveData newGameData = new();
        int slot = GetSaveCount() + 1; // Determine the next available save slot
        // Initialize new game data here
        Save(slot, newGameData);
    }
}
