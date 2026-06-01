using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Footsteps")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    private float footstepTimer;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraHolder;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    public float staminaRegenDelay = 2f;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public float bobSpeed = 8f;
    public float bobAmount = 0.05f;
    public float sprintBobMultiplier = 1.5f;

    private float defaultCameraY;
    private float bobTimer;

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
        defaultCameraY = cameraHolder.localPosition.y;
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
        HandleHeadBob();
        HandleFootsteps();
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

    void HandleHeadBob()
    {
        if (!enableHeadBob)
            return;

        bool isMoving =
            moveInput.magnitude > 0.1f &&
            characterController.isGrounded;

        if (isMoving)
        {
            float currentBobSpeed =
                isSprinting
                    ? bobSpeed * sprintBobMultiplier
                    : bobSpeed;

            bobTimer +=
                Time.deltaTime * currentBobSpeed;

            Vector3 camPos =
                cameraHolder.localPosition;

            camPos.y =
                defaultCameraY +
                Mathf.Sin(bobTimer) * bobAmount;

            cameraHolder.localPosition =
                camPos;
        }
        else
        {
            bobTimer = 0;

            Vector3 camPos =
                cameraHolder.localPosition;

            camPos.y =
                Mathf.Lerp(
                    camPos.y,
                    defaultCameraY,
                    Time.deltaTime * 5f
                );

            cameraHolder.localPosition =
                camPos;
        }

    }
    void HandleFootsteps()
    {
        bool isMoving =
            moveInput.magnitude > 0.1f &&
            characterController.isGrounded;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepClip;
                footstepSource.loop = true;
                footstepSource.Play();
            }

            footstepSource.pitch =
                isSprinting ? 1.3f : 1f;
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }
}