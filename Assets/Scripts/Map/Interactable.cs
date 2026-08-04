using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Represents an interactable object in the game world.
/// When the player interacts with this object (via PlayerController.OnInteract(context)), it will invoke the specified UnityEvent.
/// Below are some generic functions that can be used for common interactions, such as saving the game or loading a level.
/// Remember to not directly reference the player and game manager GameObjects in scenes, 
/// but instead use PlayerController.Instance and GameManager.Instance to access the singleton instances.
/// </summary>
public class Interactable : MonoBehaviour
{
    public UnityEvent onInteraction;

    public void Interact()
    {
        onInteraction.Invoke();
    }
    //generic functions
    public void Save()
    {
        SaveManager.Instance.Save();
        Debug.Log("Saved game");
    }
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }
}