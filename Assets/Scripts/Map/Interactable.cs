using UnityEngine;
using UnityEngine.Events;

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
}