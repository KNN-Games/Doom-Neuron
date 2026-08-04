using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float holdDuration = 2f;
    public UnityEvent onHoldComplete;
    private float holdTimer;
    private bool holding;
    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (!holding) return;

        holdTimer += Time.unscaledDeltaTime;
        image.fillAmount = holdTimer / holdDuration;
        if (holdTimer >= holdDuration)
        {
            holding = false;
            holdTimer = 0f;

            onHoldComplete.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holding = true;
        holdTimer = 0f;
        image.fillAmount = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;
        holdTimer = 0f;
        image.fillAmount = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        holding = false;
        holdTimer = 0f;
        image.fillAmount = 0;
    }
}