using UnityEngine;

public class HorrorFPSController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;

    private float currentSpeed;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    public float staminaDrain = 20f;
    public float staminaRecovery = 15f;

    public float sprintCooldown = 1f;

    private float cooldownTimer;

    [Header("Gravity")]
    public float gravity = -9.81f;

    private Vector3 velocity;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    public Transform cameraHolder;

    private float xRotation;

    [Header("Crouch")]
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransitionSpeed = 8f;

    private bool isCrouching;

    [Header("Head Bobbing")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    private float bobTimer;

    private Vector3 originalCamPos;

    private Vector3 move;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = maxStamina;

        currentSpeed = walkSpeed;

        originalCamPos = cameraHolder.localPosition;
    }

    void Update()
    {
        Look();
        Move();
        HandleCrouch();
        HeadBob();
    }

    void Look()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivity *
            Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera holder
        cameraHolder.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player body
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move =
            transform.right * x +
            transform.forward * z;

        bool isMoving =
            move.magnitude > 0.1f;

        bool tryingToSprint =
            Input.GetKey(KeyCode.LeftShift);

        bool canSprint =
            currentStamina > 0 &&
            cooldownTimer <= 0 &&
            !isCrouching;

        // Sprinting
        if (tryingToSprint &&
            isMoving &&
            canSprint)
        {
            currentSpeed = sprintSpeed;

            currentStamina -=
                staminaDrain * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;

                cooldownTimer = sprintCooldown;
            }
        }
        else
        {
            currentSpeed =
                isCrouching ?
                crouchSpeed :
                walkSpeed;

            currentStamina +=
                staminaRecovery * Time.deltaTime;

            currentStamina =
                Mathf.Clamp(
                    currentStamina,
                    0,
                    maxStamina
                );
        }

        // Cooldown
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        controller.Move(
            move *
            currentSpeed *
            Time.deltaTime
        );

        // Gravity
        if (controller.isGrounded &&
            velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(
            velocity * Time.deltaTime
        );
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        float targetHeight =
            isCrouching ?
            crouchHeight :
            normalHeight;

        controller.height = Mathf.Lerp(
            controller.height,
            targetHeight,
            crouchTransitionSpeed *
            Time.deltaTime
        );
    }

    void HeadBob()
    {
        bool isMoving =
            move.magnitude > 0.1f &&
            controller.isGrounded;

        if (isMoving)
        {
            // Faster while sprinting
            float currentBobSpeed =
                currentSpeed == sprintSpeed
                ? bobSpeed * 1.6f
                : bobSpeed;

            // Stronger while sprinting
            float currentBobAmount =
                currentSpeed == sprintSpeed
                ? bobAmount * 1.3f
                : bobAmount;

            bobTimer +=
                Time.deltaTime *
                currentBobSpeed;

            Vector3 bobPosition =
                originalCamPos;

            // ONLY vertical movement
            bobPosition.y +=
                Mathf.Sin(bobTimer) *
                currentBobAmount;

            cameraHolder.localPosition =
                Vector3.Lerp(
                    cameraHolder.localPosition,
                    bobPosition,
                    Time.deltaTime * 10f
                );
        }
        else
        {
            bobTimer = 0;

            cameraHolder.localPosition =
                Vector3.Lerp(
                    cameraHolder.localPosition,
                    originalCamPos,
                    Time.deltaTime * 8f
                );
        }
    }
}