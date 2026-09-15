using UnityEngine;

/// <summary>
/// The main player Script.
/// Handles player movement, looking around, and interaction with objects in the game world. Also handles the camera and some other generic player functions.
/// Remember to not directly reference the player GameObject in scenes, but instead use PlayerController.Instance to access the singleton instance.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Transform))]
public class PlayerController : Singleton<PlayerController>
{
    [HideInInspector] public Transform playerTransform;
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f; // Earth gravity
    [Header("References")]
    public Camera camera; // As in: player camera
    [SerializeField] private GameObject interactionText;
    private CharacterController characterController;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation;
    private Interactable interactable;
    private float lookSensitivity = 1f; 
    private int lookInvertY = 1; // works only on gamepad, 1f = normal, -1f = inverted
    // Get these from settings
    [HideInInspector] public float mouseSensitivity = 1f;
    [HideInInspector] public float gamepadSensitivity = 1f;
    [HideInInspector] public bool invertGamepadY = false;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        InputManager.Instance.UpdateCursorState();
        camera.fieldOfView = GameManager.Instance.fov;
        // These are also changed if player does stuff in OptionsMenu during gameplay (not in main menu)
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", 10f) / 10;
        gamepadSensitivity = PlayerPrefs.GetFloat("gamepadSensitivity", 10f) / 10;
        invertGamepadY = PlayerPrefs.GetInt("invertGamepadY", 0) == 1;

        UpdateSensitivity();
    }
    private void Update()
    {
        if (PlayerUI.Instance.isPaused) return;

        // WASD movement
        Vector3 movement = transform.right * moveInput.x + transform.forward * moveInput.y;
        movement.Normalize();
        characterController.Move(moveSpeed * Time.deltaTime * movement);

        // Gravity
        if (characterController.isGrounded && velocity.y < 0) // Apparently this prevents problems with floor collision
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        // Looking around
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity * lookInvertY;

        transform.Rotate(Vector3.up * mouseX); // left-right
        xRotation -= mouseY; // up-down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Handle interaction with objects in sight
        Ray interactRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Raycast in front of the player camera (center of the screen)
        Debug.DrawRay(interactRay.origin, interactRay.direction * 100f, Color.red); // Debug ray that points where player is looking
        if (Physics.Raycast(interactRay, out RaycastHit hit, 3f) && hit.collider.TryGetComponent(out interactable))
        {
            interactionText.SetActive(true);
        }
        else
        {
            interactionText.SetActive(false);
            interactable = null;
        }
    }
    // Functions used by InputManager.cs
    public void Jump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    public void Interact()
    {
        if (interactable == null) return;
        interactable.Interact();
    }
    public void UpdateSensitivity()
    {
        bool isGamepad = InputManager.Instance.CurrentDevice == "Gamepad";
        lookSensitivity = isGamepad ? gamepadSensitivity : mouseSensitivity;
        lookInvertY = (int)((isGamepad && invertGamepadY) ? -1f : 1f);
        Debug.Log("Updated sensitivity");
    }
    // Used to get and set player location
    public void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        xRotation = rotation.x;
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    public Vector3 GetRotation()
    {
        return new Vector3(xRotation, transform.eulerAngles.y, 0f);
    }
}
