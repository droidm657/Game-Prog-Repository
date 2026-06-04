using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BasicMonsterAI : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    [Header("Player")]
    public Transform player;

    [Header("Vision")]
    public Transform eyePoint;

    [Header("Detection")]
    public float detectionRange = 25f;
    public float flashlightDetectRange = 40f;
    public float loseInterestTime = 6f;

    [Header("Movement")]
    public float patrolSpeed = 4f;
    public float chaseSpeed = 8f;
    public float pointReachedDistance = 1f;

    [Header("Kill Settings")]
    public float killDistance = 2f;
    public float attackDuration = 3f;

    [Header("Stun")]
    public bool isStunned = false;

    [Header("Audio")]
    public AudioSource monsterAudio;
    public AudioClip patrolGrowl;
    public AudioClip spottedRoar;

    private float growlTimer;
    private bool roarPlayed;

    private NavMeshAgent agent;
    private Animator animator;

    private int currentPatrolIndex;

    private bool isChasing;
    private bool isKillingPlayer;

    private float lastTimeSawPlayer;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.acceleration = 60f;
        agent.angularSpeed = 1200f;
        agent.autoBraking = true;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(
                patrolPoints[0].position
            );
        }
    }

    void Update()
    {
        if (isStunned)
            return;

        if (player == null)
            return;

        if (isKillingPlayer)
            return;

        growlTimer += Time.deltaTime;

        float distanceToPlayer =
                    Vector3.Distance(
                transform.position,
                player.position
            );

        // Kill player
        if (distanceToPlayer <= killDistance)
        {
            StartCoroutine(KillPlayer());
            return;
        }

        // Kill player
        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing)
            {
                if (
                    monsterAudio != null &&
                    spottedRoar != null
                )
                {
                    monsterAudio.PlayOneShot(
                        spottedRoar
                    );
                }

                HorrorAudioManager.Instance?.StartChaseMusic();
            }

            isChasing = true;
            lastTimeSawPlayer = Time.time;
        }

        // Flashlight detection
        if (
            SimpleFlashlight.FlashlightOn &&
            IsInFlashlightBeam()
        )
        {
            isChasing = true;
            lastTimeSawPlayer = Time.time;
        }

            // Lose interest
            if (
         isChasing &&
         Time.time - lastTimeSawPlayer >
         loseInterestTime
     )
            {
                isChasing = false;

                HorrorAudioManager.Instance?.StopChaseMusic();
            }

            if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();

            if (
                monsterAudio != null &&
                patrolGrowl != null &&
                growlTimer >= 10f
            )
            {
                monsterAudio.PlayOneShot(
                    patrolGrowl
                );

                growlTimer = 0f;
            }
        }

        UpdateAnimations();
    }

    bool IsInFlashlightBeam()
    {
        if (SimpleFlashlight.FlashlightTransform == null)
            return false;

        Vector3 directionToMonster =
            (
                transform.position -
                SimpleFlashlight.FlashlightTransform.position
            ).normalized;

        float angle =
            Vector3.Angle(
                SimpleFlashlight.FlashlightTransform.forward,
                directionToMonster
            );

        if (angle > 20f)
            return false;

        if (
            Physics.Raycast(
                SimpleFlashlight.FlashlightTransform.position,
                directionToMonster,
                out RaycastHit hit,
                flashlightDetectRange
            )
        )
        {
            if (
                hit.transform == transform ||
                hit.transform.root == transform
            )
            {
                return true;
            }
        }

        return false;
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0)
            return;

        if (
            !agent.pathPending &&
            agent.remainingDistance <= pointReachedDistance
        )
        {
            currentPatrolIndex++;

            if (
                currentPatrolIndex >=
                patrolPoints.Length
            )
            {
                currentPatrolIndex = 0;
            }

            agent.SetDestination(
                patrolPoints[
                    currentPatrolIndex
                ].position
            );
        }
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;

        agent.SetDestination(
            player.position
        );
    }

    void UpdateAnimations()
    {
        if (animator == null)
            return;

        bool moving =
            agent.velocity.magnitude > 0.1f;

        animator.SetBool(
            "iswalking",
            moving
        );
    }

    IEnumerator KillPlayer()
    {
        isKillingPlayer = true;

        agent.isStopped = true;

        // Freeze player
        PlayerController playerController =
            player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
        }

        // Camera effect
        DeathCameraEffect effect =
            FindFirstObjectByType<DeathCameraEffect>();

        if (effect != null)
        {
            yield return StartCoroutine(
                effect.ZoomToMonster(transform)
            );
        }

        // Attack animation
        if (animator != null)
        {
            animator.SetBool(
                "Isattacking",
                true
            );
        }

        yield return new WaitForSeconds(
            attackDuration
        );

        DeathPanelManager.Instance.ShowDeathScreen();
    }

    public void StunMonster(float duration)
    {
        if (!isStunned)
        {
            StartCoroutine(
                StunCoroutine(duration)
            );
        }
    }

    IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        agent.isStopped = true;

        yield return new WaitForSeconds(
            duration
        );

        agent.isStopped = false;

        isStunned = false;
    }
}

