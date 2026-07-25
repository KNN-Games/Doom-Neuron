using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    //save information
    //ADD HERE: lowest difficulty in save file (for achievements)
    public int difficulty; // Difficulty level of the game
    public float playTime; // Total playtime in seconds
    public DateTime lastPlayed; // Last time the game was played
    //game state information
    public string sceneName; // Current scene
    public Vector3 playerPosition;
    public int playerHealth;
}
