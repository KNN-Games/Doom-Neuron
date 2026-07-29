using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float playTime; // Total playtime in seconds
    public float lastSaved;
    public int difficulty; // current difficulty level
    public int saveSlot; // As in: current save slot
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        playTime += Time.deltaTime;
        lastSaved += Time.deltaTime; //SaveManager.Save resets this
    }
}
