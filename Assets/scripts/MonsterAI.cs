using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    public Transform[] patrolPoints;

    private NavMeshAgent agent;

    [Header("Detection")]
    public float detectionRange = 10f;

    public float loseRange = 15f;

    private int currentPoint;

    private bool chasingPlayer = false;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2f;

    public float chaseSpeed = 7f;

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip patrolGrowl;

    public AudioClip screechClip;

    private bool hasScreeched = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Patrol speed
        agent.speed = patrolSpeed;

        GoToNextPoint();

        // Start growl sound
        audioSource.clip = patrolGrowl;

        audioSource.loop = true;

        audioSource.Play();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        // PLAYER DETECTION WITH LINE OF SIGHT
        if (distanceToPlayer <= detectionRange)
        {
            Vector3 directionToPlayer =
                (player.position - transform.position).normalized;

            RaycastHit hit;

            if (Physics.Raycast(
                transform.position + Vector3.up,
                directionToPlayer,
                out hit,
                detectionRange))
            {
                // Monster only sees player if no wall blocks vision
                if (hit.transform.CompareTag("Player"))
                {
                    if (!chasingPlayer)
                    {
                        StartChaseSound();
                    }

                    chasingPlayer = true;

                    // Speed up while chasing
                    agent.speed = chaseSpeed;
                }
            }
        }

        // Lose Player
        if (distanceToPlayer >= loseRange)
        {
            if (chasingPlayer)
            {
                ReturnToPatrolSound();
            }

            chasingPlayer = false;

            // Slow back down
            agent.speed = patrolSpeed;
        }

        // Chase Player
        if (chasingPlayer)
        {
            agent.destination = player.position;
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        agent.destination = patrolPoints[currentPoint].position;

        currentPoint++;

        if (currentPoint >= patrolPoints.Length)
        {
            currentPoint = 0;
        }
    }

    void StartChaseSound()
    {
        audioSource.Stop();

        audioSource.loop = false;

        audioSource.PlayOneShot(screechClip);

        hasScreeched = true;
    }

    void ReturnToPatrolSound()
    {
        audioSource.Stop();

        audioSource.clip = patrolGrowl;

        audioSource.loop = true;

        audioSource.Play();

        hasScreeched = false;
    }
}