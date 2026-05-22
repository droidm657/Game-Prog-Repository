using UnityEngine;

public class FootstepSystem : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip walkClip;
    public AudioClip sprintClip;
    public AudioClip crouchClip;

    [Header("Timing")]
    public float walkStepDelay = 0.5f;
    public float sprintStepDelay = 0.3f;
    public float crouchStepDelay = 0.8f;

    private CharacterController controller;

    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        if (!controller.isGrounded) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool isMoving = horizontal != 0 || vertical != 0;

        if (!isMoving) return;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0)
        {
            if (isSprinting)
            {
                audioSource.PlayOneShot(sprintClip);
                stepTimer = sprintStepDelay;
            }
            else if (isCrouching)
            {
                if (crouchClip != null)
                {
                    audioSource.PlayOneShot(crouchClip);
                }
                else
                {
                    audioSource.PlayOneShot(walkClip);
                }

                stepTimer = crouchStepDelay;
            }
            else
            {
                audioSource.PlayOneShot(walkClip);
                stepTimer = walkStepDelay;
            }
        }
    }
}