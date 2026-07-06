using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsPanel; // Reference to the credits panel GameObject
    public void StartCreditsSequence()
    {
        // Start the credits sequence
        creditsPanel.SetActive(true);
    }
}
