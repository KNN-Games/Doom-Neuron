using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Makes it so Hover == Select
/// Used in UI elements.
/// </summary>
public class SelectOnHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}