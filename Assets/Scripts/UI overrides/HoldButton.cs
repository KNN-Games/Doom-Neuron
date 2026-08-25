using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] private float holdDuration;
    [SerializeField] private UnityEvent onHoldComplete;
    private float holdTimer;
    private bool holding;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.fillAmount = 0f;
    }

    private void Update()
    {
        if (!holding) return;

        // Assume the button is being held down
        holdTimer += Time.unscaledDeltaTime;
        image.fillAmount = holdTimer / holdDuration;

        if (holdTimer >= holdDuration)
        {
            CompleteHold();
        }
    }

    //--- MOUSE INPUT HANDLERS ---
    public void OnPointerDown(PointerEventData eventData)
    {
        StartHold();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHold();
    }
    //--- KEYBOARD/GAMEPAD INPUT HANDLERS ---
    // TO DO: OnSubmit cannot handle holding, we need to find another way to handle this.
    // I tried using PlayerInput but Unity throws a warning:
    // Cannot find matching control scheme for DeleteButton (all control schemes are already paired to matching devices)
    public void OnSubmit(BaseEventData eventData)
    {
        StartHold();
    }
    public void OnDeselect(BaseEventData eventData)
    {
        ResetHold();
    }
    //---INTERNAL METHODS---
    private void StartHold()
    {
        if (holding) return;

        holding = true;
        holdTimer = 0f;
        image.fillAmount = 0f;
    }
    private void ResetHold()
    {
        if (!holding) return;

        holding = false;
        holdTimer = 0f;
        image.fillAmount = 0f;
    }
    private void CompleteHold()
    {
        holding = false;
        holdTimer = 0f;
        image.fillAmount = 0f;
        onHoldComplete.Invoke();
    }
}