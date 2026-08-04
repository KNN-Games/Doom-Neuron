using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Transform))]
public class PlayerController : Singleton<PlayerController>
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f; // Earth gravity
    [Header("Mouse")]
    public float mouseSensitivity = 0.5f;
    [Header("References")]
    public Camera camera; // As in: player camera
    public GameObject interactionText;
    [HideInInspector] public Transform playerTransform;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation;
    private Interactable interactable;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        if(PlayerUI.Instance.isPaused) return;

        // Debug ray that points where player is looking
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

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
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX); //left-right
        xRotation -= mouseY; //up-down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Handle interaction with objects in sight
        Ray interactRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Raycast in front of the player camera (center of the screen)
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
    //---HANDLE INPUT ACTIONS---
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(!context.started || interactable == null) return;
        interactable.Interact();
    }
}
