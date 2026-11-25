using System;
using System.IO;
using UnityEngine;

public class SettingsService
{
    private static SettingsService instance; // Not a regular singleton because this is not a monoBehaviour
    public static SettingsService Instance => instance ??= new SettingsService();

    private static string FilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    public GameSettings Current { get; private set; }
    public event Action<GameSettings, bool> Changed; // bool = isPreview

    private SettingsService()
    {
        Current = LoadFromDisk();
    }

    // Runs before any scene is loaded, so settings are ready before the first Awake() in the game
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        _ = Instance;
    }

    // Clear on leaving play mode 
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        instance = null;
    }

    private static GameSettings LoadFromDisk()
    {
        var settings = new GameSettings();
        try
        {
            if (File.Exists(FilePath))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(FilePath), settings);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not load settings, using defaults: {e.Message}");
            settings = new GameSettings(); // Throw away anything that was half-loaded
        }
        settings.Validate(); // Always, also when there was no file
        return settings;
    }

    private void WriteToDisk()
    {
        try
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(Current, true));
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not save settings: {e.Message}");
        }
    }


    public void Preview(GameSettings draft) => Changed?.Invoke(draft, true);
    public void Revert() => Changed?.Invoke(Current, true);
    public void Save(GameSettings draft)
    {
        draft.Validate();
        Current = draft.Clone();
        WriteToDisk();
        Changed?.Invoke(Current, false);
    }
}