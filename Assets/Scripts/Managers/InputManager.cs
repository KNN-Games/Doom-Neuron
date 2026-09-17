using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the PlayerInput component. Make sure that component uses functions only from this script
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    public string CurrentDevice { get; private set; }
    [HideInInspector] public PlayerInput playerInput;
    private bool IsPlayerPresent => playerController != null;
    private PlayerController playerController;
    private PlayerUI playerUI;

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
        SetActiveInputMap("UI"); // because you are most likely in the main menu
        // Try to find player
        if (!IsPlayerPresent)
        {
            GameObject potentialPlayer = GameObject.FindWithTag("Player");
            if (potentialPlayer != null)
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
                //Debug.Log("Gameplay map enabled");
                break;
            case "UI":
                playerInput.actions.FindActionMap("UI").Enable();
                playerInput.actions.FindActionMap("Gameplay").Disable();
                //Debug.Log("UI map enabled");
                break;
            default:
                Debug.LogError("Action map not found");
                break;
        }
    }
    public void UpdateCursorState()
    {
        bool showCursor = (PlayerUI.Instance == null || PlayerUI.Instance.isPaused) && CurrentDevice != "Gamepad";
        Cursor.lockState = showCursor ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = showCursor;
    }
    //---HANDLE INPUT ACTIONS---
    // Special functions
    public void OnDeviceLost()
    {
        if (IsPlayerPresent)
        {
            PlayerUI.Instance.OpenDeviceLostPrompt();
        }
        Debug.Log("Device lost");
    }
    public void OnDeviceRegained()
    {
        Debug.Log("Device regained");
    }
    public void OnDeviceChange()
    {
        if (playerInput == null) return; // Prevents errors if playerInput is null (before Awake()??? weird)
        if (playerInput.currentControlScheme == CurrentDevice) return; // Prevent spamming the log if the device is the same as last time

        CurrentDevice = playerInput.currentControlScheme;
        UpdateCursorState();
        if (IsPlayerPresent)
        {
            PlayerController.Instance.UpdateSensitivity();
        }

        Debug.Log($"Device change: {CurrentDevice}");
    }
    // Gameplay map - these do not check if player does not exist so that Unity throws an error if u forget to disable this map when player doesn't exist.
    public void OnMove(InputAction.CallbackContext context)
    {
        playerController.moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started) return;
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
    public void OnConfirm(InputAction.CallbackContext context) // TO DO: If this function gets too long replace this with something more optimized! And by that I means change it to UnityEvents
    {
        if (!context.started) return;
        if (CheatsManager.Instance.IsConsoleActive) // If cheats menu is open: Submit command
        {
            CheatsManager.Instance.SubmitCommand();
            return;
        }
        if (OptionsMenu.Instance.IsOptionsMenuOpen) // If options menu is open: Save changes
        {
            OptionsMenu.Instance.SaveChanges();
            return;
        }
        if (MainMenu.Instance.isInSplashScreen) // If splash screen is open: skip it
        {
            MainMenu.Instance.EndSplashScreen();
            return;
        }
    }
    // Global map
    public void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.started || !IsPlayerPresent || CheatsManager.Instance.IsConsoleActive) return;
        playerUI.TogglePauseMenu();
    }
    public void OnToggleConsole(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        CheatsManager.Instance.ToggleConsole();
    }
}
