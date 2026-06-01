using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    public float staminaRegenDelay = 2f;

    private CharacterController characterController;
    private float verticalRotation = 0f;
    private Vector3 velocity;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool isSprinting = false;
    private float currentStamina;
    private float regenDelayTimer = 0f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        currentStamina = maxStamina;
    }

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
    void Update()
    {
        isSprinting = Keyboard.current.leftShiftKey.isPressed;
        HandleMouseLook();
        HandleMovement();
        HandleGravity();
        HandleStamina();
    }

    void HandleMovement()
    {
        // Can only sprint if has stamina and is moving
        bool canSprint = isSprinting && currentStamina > 0 && moveInput != Vector2.zero;
        float speed = canSprint ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        characterController.Move(move * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);
        verticalRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleStamina()
    {
        bool canSprint = isSprinting && moveInput != Vector2.zero;

        if (canSprint && currentStamina > 0)
        {
            // Drain stamina while sprinting
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            regenDelayTimer = staminaRegenDelay;
        }
        else
        {
            // Regen delay countdown
            if (regenDelayTimer > 0)
            {
                regenDelayTimer -= Time.deltaTime;
            }
            else
            {
                // Regen stamina
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }

        // Update UI
        UIManager.Instance?.UpdateStaminaBar(currentStamina / maxStamina);
    }

    public float GetStaminaPercent() => currentStamina / maxStamina;
}