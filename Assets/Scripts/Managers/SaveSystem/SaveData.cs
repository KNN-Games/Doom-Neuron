using System;
using UnityEngine;

/// <summary>
/// Stores all save slot info
/// </summary>
[Serializable]
public class SaveData
{
    // Do not add a constructor! It will break SaveManager.Load()
    //ADD HERE: lowest difficulty in save file (for achievements)
    public int saveSlot; // Save slot number
    public Difficulty difficulty; // Difficulty level of the game
    public float playTime; // Total playtime in seconds
    public long lastPlayed; // Last time the game was played in ticks
    // Game state information
    public string sceneName; // Current scene
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    public void PrintSaveInfo()
    {
        Debug.Log(
        $"Save slot {saveSlot} values:\n" +
        $"difficulty: {difficulty}\n" +
        $"playTime: {playTime}\n" +
        $"lastPlayed: {lastPlayed}\n" +
        $"sceneName: {sceneName}\n" +
        $"playerLocation: {playerPosition}\n" +
        $"playerRotation: {playerRotation}");
    }
}
