using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    [Header("REFERENCES")]
    public Transform player;

    public FlashlightSystem flashlightSystem;

    public Transform[] patrolPoints;

    private NavMeshAgent agent;

    [Header("DETECTION")]
    public float detectionRange = 10f;

    public float flashlightDetectionRange = 25f;

    public float flashlightDetectionDistance = 35f;

    public float closeDetectionRange = 2.5f;

    public float memoryDuration = 6f;

    public float searchDuration = 30f;

    [Range(0, 360)]
    public float fieldOfView = 90f;

    public float crouchDetectionMultiplier = 0.4f;

    private float memoryTimer;

    private float searchTimer;

    private Vector3 lastKnownPosition;

    private bool chasingPlayer = false;

    private bool searching = false;

    private bool stunned = false;

    private bool hasDetectedPlayer = false;

    // SOUND REACTION
    private bool reactingToNoise = false;

    private float reactionTimer;

    [Header("HEARING")]
    public float walkingHearingDistance = 30f;

    public float sprintHearingDistance = 60f;

    public float crouchHearingDistance = 5f;

    [Header("MOVEMENT")]
    public float patrolSpeed = 2f;

    public float chaseSpeed = 7f;

    public float maxChaseSpeed = 12f;

    public float chaseAcceleration = 0.5f;

    public float searchSpeed = 4f;

    [Header("IDLE")]
    public float minIdleTime = 15f;

    public float maxIdleTime = 20f;

    public float lookAroundSpeed = 3f;

    private bool idling = false;

    private float idleTimer;

    private Quaternion targetRotation;

    private int currentPoint = -1;

    [Header("FLASHLIGHT")]
    public float stunDuration = 3f;

    [Header("AUDIO")]
    public AudioSource audioSource;

    public AudioClip patrolGrowl;

    public AudioClip screechClip;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent.isOnNavMesh)
        {
            agent.speed = patrolSpeed;

            GoToNextPoint();
        }
    }

    void Update()
    {
        if (!agent.isOnNavMesh || stunned)
            return;

        // SOUND REACTION
        if (reactingToNoise)
        {
            reactionTimer -= Time.deltaTime;

            // Pause briefly
            if (reactionTimer <= 0)
            {
                reactingToNoise = false;

                // START INVESTIGATION
                searching = true;

                agent.speed = searchSpeed;

                agent.ResetPath();

                agent.SetDestination(lastKnownPosition);
            }

            return;
        }

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        bool canSeePlayer =
            CheckLineOfSight();

        // FLASHLIGHT DETECTION
        if (flashlightSystem != null)
        {
            if (flashlightSystem.flashlightLight.enabled)
            {
                Vector3 directionToPlayer =
                    (player.position - transform.position).normalized;

                float flashlightDistance =
                    Vector3.Distance(
                        transform.position,
                        player.position
                    );

                // STRICT FRONT CHECK
                float dot =
                    Vector3.Dot(
                        transform.forward,
                        directionToPlayer
                    );

                // ONLY FRONT VISION
                bool playerInFront = dot > 0.5f;

                if (playerInFront &&
                    flashlightDistance <= flashlightDetectionDistance)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(
                        transform.position + Vector3.up,
                        directionToPlayer,
                        out hit,
                        flashlightDetectionDistance))
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            // ONLY INVESTIGATE
                            // DON'T INSTANTLY CHASE

                            if (!chasingPlayer)
                            {
                                // REMEMBER POSITION
                                lastKnownPosition =
                                    player.position;

                                // TURN TOWARD PLAYER
                                Vector3 lookDirection =
                                    player.position -
                                    transform.position;

                                lookDirection.y = 0;

                                Quaternion lookRotation =
                                    Quaternion.LookRotation(
                                        lookDirection
                                    );

                                transform.rotation =
                                    Quaternion.Slerp(
                                        transform.rotation,
                                        lookRotation,
                                        2f * Time.deltaTime
                                    );

                                // START INVESTIGATION
                                if (!searching &&
                                    !reactingToNoise)
                                {
                                    agent.ResetPath();

                                    idling = false;

                                    reactingToNoise = true;

                                    reactionTimer = 0.5f;

                                    searchTimer =
                                        searchDuration;
                                }
                            }
                        }
                    }
                }
            }
        }

        // PLAYER SPOTTED
        if (hasDetectedPlayer &&
           distanceToPlayer <= detectionRange &&
           canSeePlayer)
        {
            if (!chasingPlayer)
            {
                StartChase();
            }

            chasingPlayer = true;

            searching = false;

            idling = false;

            reactingToNoise = false;

            lastKnownPosition =
                player.position;

            memoryTimer =
                memoryDuration;
        }

        // CHASE PLAYER
        if (chasingPlayer)
        {
            memoryTimer -= Time.deltaTime;

            agent.speed +=
                chaseAcceleration *
                Time.deltaTime;

            agent.speed =
                Mathf.Clamp(
                    agent.speed,
                    chaseSpeed,
                    maxChaseSpeed
                );

            Vector3 predictedPosition =
                player.position +
                player.forward * 2f;

            agent.SetDestination(
                predictedPosition
            );

            // LOST PLAYER
            if (memoryTimer <= 0)
            {
                chasingPlayer = false;

                searching = true;

                searchTimer =
                    searchDuration;

                agent.speed =
                    searchSpeed;

                agent.SetDestination(
                    lastKnownPosition
                );
            }
        }

        // SEARCH MODE
        if (searching)
        {
            SearchBehavior();
        }

        // PATROL MODE
        Patrol();
    }

    bool CheckLineOfSight()
    {
        Vector3 directionToPlayer =
            (player.position -
             transform.position).normalized;

        float angle =
            Vector3.Angle(
                transform.forward,
                directionToPlayer
            );

        if (angle > fieldOfView / 2f)
        {
            return false;
        }

        float currentDetectionRange =
            detectionRange;

        // FLASHLIGHT BOOST
        if (flashlightSystem != null)
        {
            if (flashlightSystem.flashlightLight.enabled)
            {
                currentDetectionRange =
                    flashlightDetectionRange;
            }
        }

        // CROUCH STEALTH
        HorrorFPSController crouchScript =
            player.GetComponent<HorrorFPSController>();

        if (crouchScript != null &&
           crouchScript.isCrouching)
        {
            currentDetectionRange *=
                crouchDetectionMultiplier;
        }

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // VERY CLOSE ALWAYS DETECT
        if (distance <= closeDetectionRange)
        {
            return true;
        }

        if (distance > currentDetectionRange)
        {
            return false;
        }

        RaycastHit hit;

        if (Physics.Raycast(
            transform.position + Vector3.up,
            directionToPlayer,
            out hit,
            currentDetectionRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void SearchBehavior()
    {
        searchTimer -= Time.deltaTime;

        // SEARCH AREA
        if (!agent.pathPending &&
           agent.remainingDistance <= 2f)
        {
            Vector3 randomSearch =
                lastKnownPosition +
                Random.insideUnitSphere * 12f;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomSearch,
                out hit,
                12f,
                NavMesh.AllAreas))
            {
                agent.SetDestination(
                    hit.position
                );
            }
        }

        // GIVE UP SEARCH
        if (searchTimer <= 0)
        {
            searching = false;

            hasDetectedPlayer = false;

            agent.speed =
                patrolSpeed;

            ReturnToPatrol();
        }
    }

    void Patrol()
    {
        // NEVER PATROL DURING OTHER STATES
        if (chasingPlayer ||
           searching ||
           reactingToNoise)
        {
            return;
        }

        if (patrolPoints.Length == 0)
            return;

        // IDLE
        if (idling)
        {
            idleTimer -= Time.deltaTime;

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    lookAroundSpeed *
                    Time.deltaTime
                );

            if (idleTimer <= 0)
            {
                idling = false;

                GoToNextPoint();
            }

            return;
        }

        // REACHED POINT
        if (!agent.pathPending &&
           agent.remainingDistance < 0.5f)
        {
            StartIdleBehavior();
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        int randomPoint =
            currentPoint;

        while (randomPoint ==
              currentPoint)
        {
            randomPoint =
                Random.Range(
                    0,
                    patrolPoints.Length
                );
        }

        currentPoint =
            randomPoint;

        agent.SetDestination(
            patrolPoints[currentPoint].position
        );
    }

    void StartIdleBehavior()
    {
        idling = true;

        agent.ResetPath();

        idleTimer =
            Random.Range(
                minIdleTime,
                maxIdleTime
            );

        float randomY =
            Random.Range(0f, 360f);

        targetRotation =
            Quaternion.Euler(
                0f,
                randomY,
                0f
            );

        // PATROL GROWL
        if (audioSource != null &&
           patrolGrowl != null &&
           !searching &&
           !chasingPlayer)
        {
            audioSource.clip =
                patrolGrowl;

            audioSource.loop = false;

            audioSource.pitch =
                Random.Range(0.8f, 1.1f);

            audioSource.Play();
        }
    }

    void ReturnToPatrol()
    {
        GoToNextPoint();
    }

    void StartChase()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (screechClip != null)
        {
            audioSource.PlayOneShot(
                screechClip
            );
        }
    }

    public void HearNoise(
        Vector3 noisePosition,
        float playerSpeed)
    {
        if (stunned)
            return;

        // IGNORE NOISES WHILE BUSY
        if (chasingPlayer ||
           searching ||
           reactingToNoise)
        {
            return;
        }

        float hearingDistance =
            walkingHearingDistance;

        // Sprint louder
        if (playerSpeed >= 6f)
        {
            hearingDistance =
                sprintHearingDistance;
        }

        float distanceToNoise =
            Vector3.Distance(
                transform.position,
                noisePosition
            );

        // TOO FAR
        if (distanceToNoise >
           hearingDistance)
        {
            return;
        }

        hasDetectedPlayer = true;

        // FORCE STOP EVERYTHING
        agent.ResetPath();

        idling = false;

        // STOP PATROL
        searching = false;

        reactingToNoise = true;

        // 1 SECOND PAUSE
        reactionTimer = 1f;

        // STOP AUDIO
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // REMEMBER POSITION
        lastKnownPosition =
            noisePosition;

        searchTimer =
            searchDuration;
    }

    public void StunMonster(float duration)
    {
        if (stunned)
            return;

        StartCoroutine(
            StunRoutine(duration)
        );
    }

    IEnumerator StunRoutine(float duration)
    {
        stunned = true;

        agent.isStopped = true;

        yield return new WaitForSeconds(duration);

        stunned = false;

        agent.isStopped = false;

        // RETURN TO PREVIOUS STATE
        if (chasingPlayer)
        {
            agent.speed = chaseSpeed;

            memoryTimer = memoryDuration;

            agent.SetDestination(lastKnownPosition);
        }
        else if (searching)
        {
            agent.speed = searchSpeed;

            agent.SetDestination(lastKnownPosition);
        }
        else
        {
            agent.speed = patrolSpeed;

            GoToNextPoint();
        }
    }
}