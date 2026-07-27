using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float playTime; // Total playtime in seconds
    public int difficulty; // current difficulty level
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of GameManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        playTime += Time.deltaTime;
    }
}
