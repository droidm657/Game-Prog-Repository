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

    [Header("Stun")]
    public bool isStunned = false;

    private NavMeshAgent agent;
    private int currentPatrolIndex;
    private bool isChasing;
    private float lastTimeSawPlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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

        float distanceToPlayer =
            Vector3.Distance(
                transform.position,
                player.position
            );

        // NORMAL DETECTION
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            lastTimeSawPlayer = Time.time;
        }

        // FLASHLIGHT DETECTION
        if (
            SimpleFlashlight.FlashlightOn &&
            IsInFlashlightBeam()
        )
        {
            isChasing = true;
            lastTimeSawPlayer = Time.time;
        }

        // LOSE INTEREST
        if (
            isChasing &&
            Time.time - lastTimeSawPlayer >
            loseInterestTime
        )
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    bool IsInFlashlightBeam()
    {
        if (SimpleFlashlight.FlashlightTransform == null)
            return false;

        Vector3 directionToMonster =
            (
                transform.position -
                SimpleFlashlight
                .FlashlightTransform.position
            ).normalized;

        float angle =
            Vector3.Angle(
                SimpleFlashlight
                .FlashlightTransform.forward,
                directionToMonster
            );

        if (angle > 20f)
            return false;

        if (
            Physics.Raycast(
                SimpleFlashlight
                .FlashlightTransform.position,
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
            agent.remainingDistance <=
            pointReachedDistance
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