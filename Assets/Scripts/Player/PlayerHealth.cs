using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning($"Multiple instances of {GetType().Name} detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
