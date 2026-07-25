using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float playTime; // Total playtime in seconds
    public int difficulty; // current difficulty level
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        playTime += Time.deltaTime;
    }
}
