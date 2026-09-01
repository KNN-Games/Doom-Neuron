using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void CollectData()
    {
        saveSlot = SaveManager.Instance.saveSlot;
        difficulty = GameManager.Instance.difficulty;
        playTime = GameManager.Instance.playTime;
        lastPlayed = DateTime.Now.Ticks;
        if(SceneManager.GetActiveScene().name == "MainMenu") // Are you starting a new game?
        {
            // New game defaults
            sceneName = "TestArena";
            playerPosition = Vector3.zero;
            playerRotation = Vector3.zero;
        }
        else
        {
            // Save current game state
            sceneName = SceneManager.GetActiveScene().name;
            playerPosition = PlayerController.Instance.transform.position;
            playerRotation = PlayerController.Instance.GetRotation();
        }
    }
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
