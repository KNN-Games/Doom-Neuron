using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the PlayerInput component. Make sure that component uses functions only from this script
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    private bool IsPlayerPresent => playerController != null;
    [HideInInspector] public PlayerInput playerInput;
    private PlayerController playerController;
    private PlayerUI playerUI;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        SetActiveInputMap("UI");
        // Try to find player
        if(!IsPlayerPresent)
        {
            GameObject potentialPlayer = GameObject.FindGameObjectWithTag("Player");
            if(potentialPlayer != null)
            {
                SetPlayer(potentialPlayer);
            }
        }
    }
    public void SetPlayer(GameObject player)
    {
        playerController = player.GetComponent<PlayerController>();
        playerUI = player.GetComponent<PlayerUI>();
        SetActiveInputMap("Gameplay");
    }
    // Use this instead of playerInput.SwitchCurrentActionMap() to prevent Global from being disabled
    public void SetActiveInputMap(string name)
    {
        switch (name)
        {
            case "Gameplay":
                playerInput.actions.FindActionMap("Gameplay").Enable();
                playerInput.actions.FindActionMap("UI").Disable();
                break;
            case "UI":
                playerInput.actions.FindActionMap("UI").Enable();
                playerInput.actions.FindActionMap("Gameplay").Disable();
                break;
            default:
                Debug.LogError("Action map not found");
                break;

        }
    }
    //---HANDLE INPUT ACTIONS---
    // Gameplay map - these do not check if player does not exist so that Unity throws an error if u forget to disable this map when player doesn't exist.
    public void OnMove(InputAction.CallbackContext context)
    {
        playerController.moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        playerController.Jump();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        playerController.lookInput = context.ReadValue<Vector2>();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        playerController.Interact();
    }
    // UI map
    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        CheatsManager.Instance.SubmitCommand(); // If cheats menu is open: Submit command
        if(OptionsMenu.Instance.IsOptionsMenuOpen) // If options menu is open: Save changes
        {
            OptionsMenu.Instance.SaveChanges();
        }
    }
    // Global map
    public void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.started || !IsPlayerPresent || CheatsManager.Instance.IsConsoleActive) return;
        playerUI.TogglePauseMenu();
    }
    public void OnOpenConsole(InputAction.CallbackContext context)
    {
        if (!context.started || !IsPlayerPresent) return;
        CheatsManager.Instance.ToggleConsole();
    }
}
