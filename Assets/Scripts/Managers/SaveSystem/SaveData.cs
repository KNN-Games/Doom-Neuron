using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Save information
    //ADD HERE: lowest difficulty in save file (for achievements)
    public int saveSlot; // Save slot number
    public int difficulty; // Difficulty level of the game
    public float playTime; // Total playtime in seconds
    public long lastPlayed; // Last time the game was played in ticks
    // Game state information
    public string sceneName; // Current scene
    public Vector3 playerPosition;
    public Vector3 playerRotation;
}
