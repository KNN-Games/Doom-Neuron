using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float playTime; // Total playtime in seconds
    public int difficulty; // current difficulty level
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        playTime += Time.deltaTime;
    }
}
