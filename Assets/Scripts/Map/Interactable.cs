using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteraction;
    public void Interact()
    {
        onInteraction.Invoke();
    }
}